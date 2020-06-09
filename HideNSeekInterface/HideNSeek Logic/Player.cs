using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HideNSeek.Logic
{
    public abstract class Player
    {
        #region Fields
        private string _playerName;
        private int _points;
        private ILobby _lobby;
        #endregion

        #region Properties
        public string PlayerName
        {
            get { return _playerName; }
            set { _playerName = value; }
        }
        public int Points
        {
            get { return _points; }
            private set { _points = value; }
        }
        public ILobby Lobby
        {
            get { return _lobby; }
            private set { _lobby = value; }
        }
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
            _lobby.Disconnect(this);
        }
        public void EndGame()
        {
            throw new NotImplementedException();
        }
        public void AddPoints(int points)
        {
            if (points > 0)
                _points += points;
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
