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
        #endregion

        #region Constructor
        /// <summary>
        /// Instantiates a <see cref="Player"/> for a <see cref="Lobby"/>.
        /// </summary>
        /// <param name="client">The client connection.</param>
        /// <param name="username">The user's identifier.</param>
        public Player(string username, ILobby lobby)
        {
            PlayerName = username;
            Lobby = lobby;
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
            Lobby.Disconnect(this);
        }
        /// <summary>
        /// Signals the start of the game.
        /// </summary>
        public void StartGame()
        {
            BeginGame?.Invoke();
        }
        /// <summary>
        /// Signals the end of the game.
        /// </summary>
        public void EndGame()
        {
            FinishGame?.Invoke();
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
        #endregion

        #region Methods
        #endregion

        #region Events
        public delegate void BeginGameHandler();
        public event BeginGameHandler BeginGame;

        public delegate void FinishGameHandler();
        public event FinishGameHandler FinishGame;
        #endregion
    }
}
