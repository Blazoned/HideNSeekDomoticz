using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace HideNSeek.Logic
{
    /// <summary>
    /// The <see cref="Host"/> allows a player to host a lobby.
    /// </summary>
    public class Host
    {
        #region Constants
        #endregion

        #region Fields
        #endregion

        #region Properties
        /// <summary>
        /// The game lobby.
        /// </summary>
        public Lobby Lobby { get; private set; }
        /// <summary>
        /// Gets the server address.
        /// </summary>
        public string Address { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Instantiates a <see cref="Host"/> for a <see cref="Logic.Lobby"/>.
        /// </summary>
        /// <param name="username">The user's identifier.</param>
        public Host(string username)
        {
            this.Lobby = new Lobby(username);
        }
        #endregion

        #region Functions
        /// <summary>
        /// Starts the game for every <see cref="Player"/>.
        /// </summary>
        public void StartGame()
        {
            Lobby.StartGame();
        }
        /// <summary>
        /// Sets the amount of time the hider has for hiding.
        /// </summary>
        /// <param name="seconds">The amount of seconds.</param>
        public void SetHidingTime(int seconds)
        {
            Lobby.SetHidingTime(seconds);
        }
        /// <summary>
        /// Ends the game for every <see cref="Player"/>
        /// </summary>
        public void EndGame()
        {
            Lobby.EndGame();
        }
        /// <summary>
        /// Disbands the entire lobby.
        /// </summary>
        public void DisbandLobby()
        {
            Lobby.Disconnect(Lobby.HostPlayer);
        }
        #endregion

        #region Methods
        #endregion
    }
}
