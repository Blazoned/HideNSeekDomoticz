using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HideNSeek.Logic
{
    public abstract class Player
    {
        #region Properties
        public string PlayerName { get; set; }
        public int Points { get; private set; }
        public ILobby Lobby { get; private set; }
        #endregion

        #region Constructor
        #endregion

        #region Functions
        public void SwapRole()
        {
            throw new NotImplementedException();
        }
        public void Disconnect()
        {
            Lobby.Disconnect(this);
        }
        public void EndGame()
        {
            throw new NotImplementedException();
        }
        public void AddPoints(int points)
        {
            if (points > 0)
                Points += points;
        }
        #endregion

        #region Methods
        #endregion

        #region Events
        public delegate string PlayerFoundHandler(string playerName);
        public event PlayerFoundHandler PlayerFound;
        #endregion
    }
}
