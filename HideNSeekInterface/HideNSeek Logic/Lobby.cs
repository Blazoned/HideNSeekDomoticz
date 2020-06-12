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
        #region Fields
        private int _remainingHidingTime;
        public List<Seeker> _seekers;
        public List<Hider> _hiders;
        private bool _isActive;
        #endregion

        #region Properties
        /// <summary>
        /// The amount of time a <see cref="Hider"/> has until a <see cref="Seeker"/> can look for them.
        /// </summary>
        public int HidingTime { get; private set; }

        /// <summary>
        /// The host's player object.
        /// </summary>
        public Player HostPlayer { get; private set; }

        /// <summary>
        /// Checks if the user is actually the host.
        /// </summary>
        private bool _IsHost { get { return HostPlayer != null; } }
        #endregion

        #region Constructor
        /// <summary>
        /// Initiate a lobby for players to join a host.
        /// </summary>
        public Lobby()
        {
            this._seekers = new List<Seeker>();
            this._hiders = new List<Hider>();
        }
        /// <summary>
        /// Initiate a lobby for players to host a new lobby.
        /// </summary>
        /// <param name="hostIdentifier">The username of the host of the lobby.</param>
        /// <param name="hidingTime">The amount of time a <see cref="Hider"/> has for hiding.</param>
        public Lobby(string hostIdentifier, int hidingTime = 300)
        {
            this.HidingTime = hidingTime;
            this._seekers = new List<Seeker>();
            this.HostPlayer = new Hider(hostIdentifier, this, new DomoticzHider("127.0.0.1", "8080"));
            this._hiders = new List<Hider>
            {
                this.HostPlayer as Hider
            };
        }
        #endregion

        #region Functions
        #region Lobby Functions
        /// <summary>
        /// Starts the game for every player.
        /// </summary>
        internal void StartGame()
        {
            _isActive = true;
            _remainingHidingTime = HidingTime;
            IEnumerable<Player> players = GetAllPlayers();

            foreach(Player player in players)
            {
                player.StartGame();
            }

            _ = Task.Run(() => StartHidingTime());
        }
        /// <summary>
        /// Sets the amount of time a <see cref="Hider"/> has until the <see cref="Seeker"/>s can look for them.
        /// </summary>
        /// <param name="seconds">The amount of seconds.</param>
        internal void SetHidingTime(int seconds)
        {
            if (!this._isActive)
                HidingTime = seconds;
        }
        /// <summary>
        /// Signals all <see cref="Player"/>s that the game has ended.
        /// </summary>
        internal void EndGame()
        {
            _isActive = false;
            IEnumerable<Player> players = GetAllPlayers();

            foreach (Player player in players)
            {
                player.EndGame();
            }
        }
        #endregion

        #region ILobby Interface
        /// <summary>
        /// Connect to the lobby as a new player.
        /// </summary>
        /// <param name="address">The host's ip address.</param>
        /// <param name="username">The identifier of the user for this lobby.</param>
        /// <returns>Returns a player object for the new player.</returns>
        public Player Connect(string address, string username)
        {
            IEnumerable<Player> players = GetAllPlayers();

            if (players.Count(p => p.PlayerName == username) > 0)
                return null;

            Seeker player = new Seeker(username, this);
            this._seekers.Add(player);
            PlayerConnected(player);
            return player;
        }

        /// <summary>
        /// Disconnects a player from the lobby.
        /// </summary>
        /// <param name="player">The player to disconnect.</param>
        public void Disconnect(Player player)
        {
            if (_IsHost && player.PlayerName == HostPlayer.PlayerName)
            {
                EndGame();
            }
            else
            {
                if (player as Hider != null)
                    _hiders.Remove(player as Hider);

                if (player as Seeker != null)
                    _seekers.Remove(player as Seeker);

                if (_hiders.Count(p => !p.IsFound) <= 0 || _seekers.Count == 0)
                    EndGame();

                PlayerDisconnected(player);
            }
        }

        /// <summary>
        /// Gets the remaining time for the hiders to keep hiding.
        /// </summary>
        /// <returns>Returns the time in seconds.</returns>
        public int GetRemainingHidingTime()
        {
            return _remainingHidingTime;
        }

        /// <summary>
        /// Checks if a hider is in the specified room.
        /// </summary>
        /// <param name="seeker">The seeker who is guessing.</param>
        /// <param name="playerName">The name of the hider.</param>
        /// <param name="roomName">The name of the room.</param>
        /// <returns>Returns true if the seeker has correctly guessed the room the hider was hiding in.</returns>
        public bool GuessRoom(Player seeker, string playerName, string roomName)
        {
            Hider hider = _hiders.Where(player => player.PlayerName == playerName).FirstOrDefault();
            bool? result = hider?.CheckRoom(roomName);

            if (result == true)
                seeker.AddPoints(10);

            if (_hiders.Count(p => !p.IsFound) <= 0)
                EndGame();

            return result == true;
        }


        /// <summary>
        /// Gets the position of a <see cref="Hider"/>.
        /// </summary>
        /// <param name="hiderUsername">The identifier of the hider.</param>
        /// <returns>The <see cref="Room"/> a <see cref="Hider"/> is currently in.</returns>
        public Room GetHiderLocation(string hiderName)
        {
            Hider hider = _hiders.Where(player => player.PlayerName == hiderName).FirstOrDefault();
            return hider?.GetPosition() ?? new Room();
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
        public Dictionary<string, Map> ViewMaps()
        {
            Dictionary<string, Map> maps = new Dictionary<string, Map>();

            _hiders.Where(player => !player.IsFound).ToList().ForEach((hider) => maps.Add(hider.PlayerName, hider.Map));

            return maps;
        }
        #endregion
        #endregion

        #region Methods
        /// <summary>
        /// Gets a list of all <see cref="Player"/>s in the lobby.
        /// </summary>
        /// <returns>A list of <see cref="Player"/>s</returns>
        private IEnumerable<Player> GetAllPlayers()
        {
            List<Player> players = new List<Player>();
            players.AddRange(_hiders);
            players.AddRange(_seekers);
            return players;
        }

        /// <summary>
        /// Starts the hiding timer
        /// </summary>
        private void StartHidingTime()
        {
            _remainingHidingTime = HidingTime;

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

        #region Events
        public delegate void PlayerConnectedHandler(Player player);
        public event PlayerConnectedHandler PlayerConnected;

        public delegate void PlayerDisconnectedHandler(Player player);
        public event PlayerDisconnectedHandler PlayerDisconnected;
        #endregion
    }
}
