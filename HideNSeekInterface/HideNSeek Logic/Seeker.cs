using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace HideNSeek.Logic
{
    /// <summary>
    /// A <see cref="Player"/> within a <see cref="Lobby"/> who are seeking a players of type <see cref="Hider"/>.
    /// </summary>
    public class Seeker : Player
    {
        #region Constructors
        /// <summary>
        /// Initiate a <see cref="Player"/> that seeks.
        /// </summary>
        /// <param name="client">The client connection.</param>
        /// <param name="username">The user's identifier.</param>
        public Seeker(string username) : base(username) { }
        #endregion

        #region Functions
        /// <summary>
        /// View the maps of players who are not yet found.
        /// </summary>
        /// <returns>Returns a dictionary of players and their corresponding maps.</returns>
        public Dictionary<string, Map> ViewMaps()
        {
            return Lobby.ViewMaps();
        }
        /// <summary>
        /// Guess the room a <see cref="Hider"/> is hiding in.
        /// </summary>
        /// <param name="playerName">The name of the hider.</param>
        /// <param name="room">The name of the room on the hider's map.</param>
        /// <returns>Returns true if found, else returns false.</returns>
        public bool GuessRoom(string playerName, string roomName)
        {
            return Lobby.GuessRoom(this, playerName, roomName);
        }
        #endregion
    }
}
