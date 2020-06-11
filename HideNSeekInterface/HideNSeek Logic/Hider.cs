using HideNSeek.DAL.Seeker;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace HideNSeek.Logic
{
    /// <summary>
    /// A <see cref="Player"/> within a <see cref="Lobby"/> who are hiding for the <see cref="Seeker"/> players.
    /// </summary>
    public class Hider : Player
    {
        #region Properties
        /// <summary>
        /// Gets whether or not the <see cref="Hider"/> has been found.
        /// </summary>
        public bool IsFound { get; private set; }
        /// <summary>
        /// Gets the <see cref="Hider"/>'s map.
        /// </summary>
        public Map Map { get; private set; }
        #endregion

        #region Fields
        private IHiderDAL _hiderDAL;
        #endregion

        #region Constructor
        /// <summary>
        /// Initiate a <see cref="Player"/> that hides.
        /// </summary>
        /// <param name="client">The client connection.</param>
        /// <param name="username">The user's identifier.</param>
        public Hider(TcpClient client, string username, IHiderDAL dal) : base(client, username)
        {
            BeginGame += BeginHiding;
            _hiderDAL = dal;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Notifies that the user is found.
        /// </summary>
        public void SetPlayerFound()
        {
            IsFound = true;
            PlayerFound(this.PlayerName);
        }

        public List<Room> SetMap()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Start the hiding process.
        /// </summary>
        public void StartHiding()
        {
            _hiderDAL.StartHiding();
        }
        /// <summary>
        /// Check if the <see cref="Hider"/> is in the specified room.
        /// </summary>
        /// <param name="roomName">The room name.</param>
        /// <returns>Returns true if the <see cref="Hider"/> has been found.</returns>
        public bool CheckRoom(string roomName)
        {
            bool result = _hiderDAL.GetCurrentRoom() == roomName;

            if (result)
                SetPlayerFound();

            return result;
        }
        /// <summary>
        /// Gets the room the <see cref="Hider"/> is currently in.
        /// </summary>
        /// <returns>The room the hider is in.</returns>
        public Room GetPosition()
        {
            string roomName = _hiderDAL.GetCurrentRoom();
            return new Room(roomName);
        }
        #endregion

        #region Methods
        private void BeginHiding(int hidingTime)
        {
            StartHiding();
        }
        #endregion

        #region Events
        public delegate string PlayerFoundHandler(string playerName);
        public event PlayerFoundHandler PlayerFound;
        #endregion
    }
}
