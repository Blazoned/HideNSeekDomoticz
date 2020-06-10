using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HideNSeek.Logic
{
    public class Hider : Player
    {
        #region Properties
        public bool IsFound { get; private set; }
        public Map Map { get; private set; }
        #endregion

        #region Fields
        #endregion

        #region Constructor
        /// <summary>
        /// Initiate a <see cref="Player"/> that hides.
        /// </summary>
        /// <param name="client">The client connection.</param>
        /// <param name="username">The user's identifier.</param>
        public Hider(TcpClient client, string username) : base(client, username) { }
        #endregion

        #region Functions
        public void SetPlayerFound()
        {
            IsFound = true;
            PlayerFound(this.PlayerName);
            BeginGame += BeginHiding;
        }

        public List<Room> SetMap()
        {
            throw new NotImplementedException();
        }

        public void StartHiding()
        {
            throw new NotImplementedException();
        }

        public bool CheckRoom(string roomName)
        {
            throw new NotImplementedException();
        }

        public Room GetPosition()
        {
            throw new NotImplementedException();
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
