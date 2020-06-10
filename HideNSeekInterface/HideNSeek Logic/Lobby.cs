using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace HideNSeek.Logic
{
    internal class Lobby : ILobby
    {
        #region Fields
        private int _hidingTime;
        private List<Seeker> _seekers;
        private List<Hider> _hiders;
        private Host _host;
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
        #endregion

        #region Constructor
        /// <summary>
        /// Initiate a lobby for players to join a host.
        /// </summary>
        public Lobby()
        {

        }
        /// <summary>
        /// Initiate a lobby for players to host a new lobby.
        /// </summary>
        /// <param name="host">The host of the lobby.</param>
        /// <param name="hidingTime">The amount of time a <see cref="Hider"/> has for hiding.</param>
        public Lobby(Host host, int hidingTime = 300)
        {
            this._host = host;
            this._hidingTime = hidingTime;
            this._seekers = new List<Seeker>();
            this._hiders = new List<Hider>();
        }
        #endregion

        #region Functions
        #region Lobby Functions
        /// <summary>
        /// Starts the game for every player.
        /// </summary>
        public void StartGame()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Sets the amount of time a <see cref="Hider"/> has until the <see cref="Seeker"/>s can look for them.
        /// </summary>
        /// <param name="seconds">The amount of seconds.</param>
        public void SetHidingTime(int seconds)
        {
            _hidingTime = seconds;
        }
        /// <summary>
        /// Signals all <see cref="Player"/>s that the game has ended.
        /// </summary>
        public void EndGame()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region ILobby Interface
        public Player Connect(int address)
        {
            throw new NotImplementedException();
        }

        public void Disconnect(Player player)
        {
            throw new NotImplementedException();
        }

        public int GetRemainingHidingTime()
        {
            throw new NotImplementedException();
        }

        public bool GuessRoom(Player seeker, string playerName, string roomName)
        {
            throw new NotImplementedException();
        }

        public bool IsGameActive()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, Map> ViewMaps()
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion

        #region Methods
        #endregion
    }
}
