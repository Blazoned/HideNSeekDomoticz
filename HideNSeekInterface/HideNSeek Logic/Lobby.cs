using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Threading;
using HideNSeek.DAL.Seeker;

namespace HideNSeek.Logic
{
    /// <summary>
    /// Defines a game <see cref="Lobby"/>.
    /// </summary>
    public class Lobby : ILobby
    {
        #region Constants
        const int PORT = 25555;
        #endregion

        #region Fields
        private int _hidingTime;
        private int _remainingHidingTime;
        private List<Seeker> _seekers;
        private List<Hider> _hiders;
        private bool _isActive;
        private readonly bool _isHost;
        private TcpClient _guestClient;
        private CancellationTokenSource _cancellationToken;
        #endregion

        #region Properties
        /// <summary>
        /// The amount of time a <see cref="Hider"/> has until a <see cref="Seeker"/> can look for them.
        /// </summary>
        public int HidingTime
        {
            get { return _hidingTime; }
            private set { _hidingTime = value; }
        }

        /// <summary>
        /// Checks if the user is actually the host.
        /// </summary>
        private bool _IsHost { get { return _guestClient != null || _isHost; } }
        #endregion

        #region Constructor
        /// <summary>
        /// Initiate a lobby for players to join a host.
        /// </summary>
        public Lobby()
        {
            this._isHost = false;
        }
        /// <summary>
        /// Initiate a lobby for players to host a new lobby.
        /// </summary>
        /// <param name="hostIdentifier">The username of the host of the lobby.</param>
        /// <param name="hidingTime">The amount of time a <see cref="Hider"/> has for hiding.</param>
        public Lobby(string hostIdentifier, int hidingTime = 300)
        {
            this._isHost = true;

            this._hidingTime = hidingTime;
            this._seekers = new List<Seeker>();
            this._hiders = new List<Hider>
            {
                new Hider(null, hostIdentifier, new DomoticzHider("127.0.0.1", "8080"))
            };
        }
        #endregion

        #region Functions
        #region Lobby Functions
        /// <summary>
        /// Starts the game for every player.
        /// </summary>
        public async Task StartGame()
        {
            _ = Task.Run(() => StartHidingTime());
            await SendCommandToAllRemote(ERemoteCommand.StartGame, string.Empty);
            this._isActive = true;
        }
        /// <summary>
        /// Sets the amount of time a <see cref="Hider"/> has until the <see cref="Seeker"/>s can look for them.
        /// </summary>
        /// <param name="seconds">The amount of seconds.</param>
        public void SetHidingTime(int seconds)
        {
            if (!this._isActive)
                _hidingTime = seconds;
        }
        /// <summary>
        /// Signals all <see cref="Player"/>s that the game has ended.
        /// </summary>
        public async Task EndGame()
        {
            if (_IsHost)
            {
                await SendCommandToAllRemote(ERemoteCommand.EndGame, string.Empty);
                this._isActive = false;
            }
            else
            {
                await SendCommandToHost(string.Empty, EHostCommand.EndGame, string.Empty);
                this._isActive = false;
            }
        }
        /// <summary>
        /// Handles new tcp client connections and stores them in a dictionary.
        /// </summary>
        /// <param name="client">The new tcp client.</param>
        /// <returns></returns>
        public async Task AddClientAsync(TcpClient client)
        {
            // Open stream
            using (NetworkStream ns = client.GetStream())
            {
                int success = -1;

                // Get the new client identifier
                using (StreamReader sr = new StreamReader(ns))
                {
                    string username = await sr.ReadLineAsync();

                    if (_seekers.Where(player => player.PlayerName == username).Count() == 0
                        && _hiders.Where(player => player.PlayerName == username).Count() == 0)
                    {
                        _seekers.Add(new Seeker(client, username));
                        success = HidingTime;
                    }
                }
                // Send the new client either a code of failure (-1) or the hiding time.
                using (StreamWriter sw = new StreamWriter(ns))
                {
                    sw.WriteLine(success);
                    sw.Flush();
                }
            }

            // Start listening for host commandos
            await HandleHostCommands(client);
        }
        #endregion

        #region ILobby Interface
        /// <summary>
        /// Connect to the lobby as a new player.
        /// </summary>
        /// <param name="address">The host's ip address.</param>
        /// <param name="username">The identifier of the user for this lobby.</param>
        /// <returns>Returns a player object for the new player.</returns>
        public async Task<Player> ConnectAsync(string address, string username)
        {
            using (TcpClient client = new TcpClient())
            {
                await client.ConnectAsync(IPAddress.Parse(address), PORT);

                Player player = null;

                using (NetworkStream ns = client.GetStream())
                {
                    using (StreamWriter sw = new StreamWriter(ns))
                    {
                        await sw.WriteLineAsync(username);
                        sw.Flush();
                    }

                    using (StreamReader sr = new StreamReader(ns))
                    {
                        int result = int.Parse(await sr.ReadLineAsync());

                        if (result > -1)
                        {
                            HidingTime = result;
                            player = new Seeker(client, username);
                            this._seekers.Add(player as Seeker);
                            this._guestClient = client;
                        }
                    }
                }

                // Start listening for remote commandos
                _ = Task.Run(async () => await HandleRemoteCommands(player));

                return player;
            }
        }

        /// <summary>
        /// Disconnects a player from the lobby.
        /// </summary>
        /// <param name="player">The player to disconnect.</param>
        public async Task DisconnectAsync(Player player)
        {
            // If host is handling disconnection,
            // check if disconnect was called on host or guest
            if (_IsHost)
            {
                // Disconnect all clients if called from host,
                // else remove client from lobby
                if (player.Client == null)
                {
                    foreach (Seeker seeker in _seekers)
                    {
                        if (seeker != player)
                            seeker.Disconnect();
                    }

                    foreach (Hider hider in _hiders)
                    {
                        if (hider != player)
                            hider.Disconnect();
                    }
                }
                else
                {
                    _seekers.Remove(player as Seeker);
                    _hiders.Remove(player as Hider);
                }

                _cancellationToken.Cancel();
            }
            else
            {
                // Send disconnection command
                player.Client.Close();
                await SendCommandToHost(player.PlayerName, EHostCommand.Disconnect, string.Empty);
            }
        }

        /// <summary>
        /// Gets the remaining time for the hiders to keep hiding.
        /// </summary>
        /// <returns>Returns the time in seconds.</returns>
        public async Task<int> GetRemainingHidingTimeAsync()
        {
            if (_IsHost)
            {
                return _remainingHidingTime;
            }
            else
            {
                return int.Parse(await SendCommandToHost(string.Empty, EHostCommand.GetRemainingTime, string.Empty));
            }
        }

        /// <summary>
        /// Checks if a hider is in the specified room.
        /// </summary>
        /// <param name="seeker">The seeker who is guessing.</param>
        /// <param name="playerName">The name of the hider.</param>
        /// <param name="roomName">The name of the room.</param>
        /// <returns>Returns true if the seeker has correctly guessed the room the hider was hiding in.</returns>
        public async Task<bool> GuessRoomAsync(Player seeker, string playerName, string roomName)
        {
            // If host is handling,
            // request data from hider
            if (_IsHost)
            {
                Hider hider = _hiders.Where(player => player.PlayerName == playerName).FirstOrDefault();
                return bool.Parse(await hider.SendCommand(ERemoteCommand.GuessRoom, roomName));
            }
            else
            {
                bool result = bool.Parse(await SendCommandToHost(playerName, EHostCommand.GuessRoom, roomName));

                if (result)
                {
                    await seeker.SendCommand(ERemoteCommand.AddPoints, 10.ToString());

                    // End game if all hiders are found.
                    if (_hiders.Count(hider => !hider.IsFound) <= 0)
                        _ = Task.Run(async () => await EndGame());
                }

                return result;
            }
        }


        /// <summary>
        /// Gets the position of a <see cref="Hider"/>.
        /// </summary>
        /// <param name="hiderUsername">The identifier of the hider.</param>
        /// <returns>The <see cref="Room"/> a <see cref="Hider"/> is currently in.</returns>
        public async Task<Room> GetHiderLocation(string hiderUsername)
        {
            if (_IsHost)
            {
                Hider hider = _hiders.Where(player => player.PlayerName == hiderUsername).FirstOrDefault();
                return JsonConvert.DeserializeObject<Room>(await hider.SendCommand(ERemoteCommand.GetPosition, string.Empty));
            }
            else
            {
                return JsonConvert.DeserializeObject<Room>(await SendCommandToHost(hiderUsername, EHostCommand.GetHiderPosition, string.Empty));
            }
        }

        /// <summary>
        /// Checks if the game is currently running.
        /// </summary>
        /// <returns></returns>
        public bool IsGameActive()
        {
            return _isActive;
        }


        /// <summary>
        /// Gets the maps from players who have not been found yet.
        /// </summary>
        /// <returns>Returns a dictionary with maps corresponding to players.</returns>
        public async Task<Dictionary<string, Map>> ViewMapsAsync()
        {
            // If host is handling get maps from active hider,
            // else return maps
            if (_IsHost)
            {
                Dictionary<string, Map> maps = new Dictionary<string, Map>();

                foreach (Hider hider in _hiders.Where(player => !player.IsFound))
                {
                    Map map = JsonConvert.DeserializeObject<Map>(await hider.SendCommand(ERemoteCommand.GetMap, string.Empty));
                    maps.Add(hider.PlayerName, map);
                }

                return maps;
            }
            else
            {
                return JsonConvert.DeserializeObject<Dictionary<string, Map>>
                    (await SendCommandToHost(string.Empty, EHostCommand.ViewMaps, string.Empty));
            }
        }
        #endregion
        #endregion

        #region Methods
        /// <summary>
        /// Handle incoming commands to host.
        /// </summary>
        /// <param name="client">The incoming client.</param>
        /// <returns></returns>
        private async Task HandleHostCommands(TcpClient client)
        {
            _cancellationToken = new CancellationTokenSource();

            while(_cancellationToken.IsCancellationRequested)
            {
                using (NetworkStream ns = client.GetStream())
                {
                    string target = string.Empty;
                    string command = string.Empty;
                    string arguments = string.Empty;

                    using (StreamReader sr = new StreamReader(ns))
                    {
                        target = await sr.ReadLineAsync();
                        command = await sr.ReadLineAsync();
                        arguments = await sr.ReadLineAsync();
                    }

                    using (StreamWriter wr = new StreamWriter(ns))
                    {
                        await wr.WriteLineAsync(await ExecuteHostCommands(target, command, arguments));
                    }
                }
            }
        }
        /// <summary>
        /// Execute incomming commands on host
        /// </summary>
        /// <param name="target">The user target.</param>
        /// <param name="command">The command to send.</param>
        /// <param name="arguments">Additional arguments to send.</param>
        /// <returns>Returns the command results.</returns>
        private async Task<string> ExecuteHostCommands(string target, string command, string arguments)
        {
            if (!Enum.TryParse(command, out EHostCommand hostCommand))
                return string.Empty;

            switch (hostCommand)
            {
                case EHostCommand.Disconnect:
                    Player player = _hiders.FirstOrDefault(p => p.PlayerName == target) ?? (Player)_seekers.FirstOrDefault(p => p.PlayerName == target);
                    player?.Client.Close();
                    break;
                case EHostCommand.EndGame:
                    await EndGame();
                    break;
                case EHostCommand.GetRemainingTime:
                    return _remainingHidingTime.ToString();
                case EHostCommand.GuessRoom:
                    Hider hider = _hiders.FirstOrDefault(p => p.PlayerName == target);
                    bool result = await hider?.SendCommand(ERemoteCommand.GetPosition, string.Empty) == arguments;
                    return result.ToString();
                case EHostCommand.ViewMaps:
                    return JsonConvert.SerializeObject(await ViewMapsAsync());
                default:
                    return string.Empty;
            }

            return string.Empty;
        }
        /// <summary>
        /// Send a command to host.
        /// </summary>
        /// <param name="target">The user target.</param>
        /// <param name="command">The command to send.</param>
        /// <param name="arguments">Additional arguments to send.</param>
        private async Task<string> SendCommandToHost(string target, EHostCommand command, string arguments)
        {
            if (_IsHost)
                return string.Empty;

            using (NetworkStream ns = _guestClient.GetStream())
            {
                using (StreamWriter sw = new StreamWriter(ns))
                {
                    await sw.WriteLineAsync(target);
                    await sw.WriteLineAsync(command.ToString());
                    await sw.WriteLineAsync(arguments);
                    sw.Flush();
                }
                using (StreamReader sr = new StreamReader(ns))
                {
                    return await sr.ReadLineAsync();
                }
            }
        }
        /// <summary>
        /// Handle incoming commands to remote connection.
        /// </summary>
        /// <param name="player">The remote player.</param>
        /// <returns></returns>
        private async Task HandleRemoteCommands(Player player)
        {
            _cancellationToken = new CancellationTokenSource();

            while (_cancellationToken.IsCancellationRequested)
            {
                using (NetworkStream ns = player.Client.GetStream())
                {
                    string command = string.Empty;
                    string arguments = string.Empty;

                    using (StreamReader sr = new StreamReader(ns))
                    {
                        command = await sr.ReadLineAsync();
                        arguments = await sr.ReadLineAsync();
                    }

                    using (StreamWriter wr = new StreamWriter(ns))
                    {
                        await wr.WriteLineAsync(ExecuteRemoteCommands(player, command, arguments));
                    }
                }
            }
        }
        /// <summary>
        /// Execute incomming commands on remote
        /// </summary>
        /// <param name="player">The remote player.</param>
        /// <param name="command">The command to send.</param>
        /// <param name="arguments">Additional arguments to send.</param>
        /// <returns>Returns the command results.</returns>
        private string ExecuteRemoteCommands(Player player, string command, string arguments)
        {
            if (!Enum.TryParse(command, out ERemoteCommand remoteCommand))
                return string.Empty;

            Hider hider = player as Hider;

            switch (remoteCommand)
            {
                case ERemoteCommand.StartGame:
                    player.StartGame();
                    break;
                case ERemoteCommand.EndGame:
                    player.EndGame();
                    break;
                case ERemoteCommand.GuessRoom:
                    return hider?.CheckRoom(arguments).ToString();
                case ERemoteCommand.AddPoints:
                    player.AddPoints(int.Parse(arguments));
                    break;
                case ERemoteCommand.GetMap:
                    return JsonConvert.SerializeObject(hider?.Map);
                case ERemoteCommand.GetPosition:
                    return JsonConvert.SerializeObject(hider?.GetPosition());
                default:
                    return string.Empty;
            }

            return string.Empty;
        }
        /// <summary>
        /// Send a command to all client connections.
        /// </summary>
        /// <param name="command">The command to send.</param>
        /// <param name="arguments">Additional arguments to send.</param>
        private async Task SendCommandToAllRemote(ERemoteCommand command, string arguments)
        {
            foreach (Hider hider in _hiders)
                await hider.SendCommand(command, arguments);
            foreach (Seeker seeker in _seekers)
                await seeker.SendCommand(command, arguments);
        }
        /// <summary>
        /// Starts the hiding timer
        /// </summary>
        private void StartHidingTime()
        {
            _remainingHidingTime = _hidingTime;

            while (_remainingHidingTime > 0)
            {
                _remainingHidingTime -= 1;
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Adds points to seeker.
        /// </summary>
        /// <param name="username">User's identifier.</param>
        /// <param name="points">The amount of points.</param>
        private void AddPoints(string username, int points)
        {
            _seekers.Where(player => player.PlayerName == username)
                    .FirstOrDefault()
                    .AddPoints(points);
        }
        #endregion
    }
}
