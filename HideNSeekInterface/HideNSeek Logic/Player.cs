using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace HideNSeek.Logic
{
    /// <summary>
    /// Defines the super class of a generic <see cref="Player"/>.
    /// </summary>
    public abstract class Player
    {
        #region Properties
        /// <summary>
        /// The users identity.
        /// </summary>
        public string PlayerName { get; private set; }
        /// <summary>
        /// The player's points.
        /// </summary>
        public int Points { get; private set; }
        /// <summary>
        /// The currently entered lobby.
        /// </summary>
        public ILobby Lobby { get; private set; }
        /// <summary>
        /// The client connection to the host.
        /// </summary>
        public TcpClient Client { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Instantiates a <see cref="Player"/> for a <see cref="Lobby"/>.
        /// </summary>
        /// <param name="client">The client connection.</param>
        /// <param name="username">The user's identifier.</param>
        public Player(TcpClient client, string username)
        {
            Client = client;
            PlayerName = username;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Swaps the users role in the lobby, toggling between seeker and hider.
        /// </summary>
        public void SwapRole()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Disconnects the <see cref="Player"/> from the lobby.
        /// </summary>
        public void Disconnect()
        {
            Lobby.DisconnectAsync(this);
        }
        /// <summary>
        /// Signals the start of the game.
        /// </summary>
        public async void StartGame()
        {
            BeginGame(await Lobby.GetRemainingHidingTimeAsync());
        }
        /// <summary>
        /// Signals the end of the game.
        /// </summary>
        public void EndGame()
        {
            FinishGame();
        }
        /// <summary>
        /// Awards points to the player.
        /// </summary>
        /// <param name="points">The amount of points.</param>
        public void AddPoints(int points)
        {
            if (points > 0)
                Points += points;
        }
        /// <summary>
        /// Send a command to client connection.
        /// </summary>
        /// <param name="target">The user target.</param>
        /// <param name="command">The command to send.</param>
        /// <param name="arguments">Additional arguments to send.</param>
        public async Task<string> SendCommand(ERemoteCommand command, string arguments)
        {
            // If no client found, player is the host
            // Then trigger local command
            // Else send command request to remote player
            if (Client == null)
                return ExecuteCommandsLocally(command, arguments);
            else
            {
                using (NetworkStream ns = Client.GetStream())
                {
                    using (StreamWriter sw = new StreamWriter(ns))
                    {
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
        }
        #endregion

        #region Methods
        /// <summary>
        /// Execute incomming commands on host player.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="arguments">Additional arguments to the command.</param>
        /// <returns>Returns the command results.</returns>
        private string ExecuteCommandsLocally(ERemoteCommand command, string arguments)
        {
            Hider hider = this as Hider;

            switch (command)
            {
                case ERemoteCommand.StartGame:
                    StartGame();
                    break;
                case ERemoteCommand.EndGame:
                    EndGame();
                    break;
                case ERemoteCommand.GuessRoom:
                    return hider?.CheckRoom(arguments).ToString();
                case ERemoteCommand.AddPoints:
                    AddPoints(int.Parse(arguments));
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
        #endregion

        #region Events
        public delegate void BeginGameHandler(int hidingTime);
        public event BeginGameHandler BeginGame;

        public delegate void FinishGameHandler();
        public event FinishGameHandler FinishGame;
        #endregion
    }
}
