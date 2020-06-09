using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HideNSeek.Logic
{
    /// <summary>
    /// An interface for a game lobby.
    /// </summary>
    public interface ILobby
    {
        /// <summary>
        /// Connect to the lobby as a new player.
        /// </summary>
        /// <param name="address">The host's ip address.</param>
        /// <returns>Returns a player object for the new player.</returns>
        Player Connect(int address);
        /// <summary>
        /// Disconnects a player from the lobby.
        /// </summary>
        /// <param name="player">The player to disconnect.</param>
        void Disconnect(Player player);
        /// <summary>
        /// Gets the remaining time for the hiders to keep hiding.
        /// </summary>
        /// <returns>Returns the time in seconds.</returns>
        int GetRemainingHidingTime();
        /// <summary>
        /// Returns whether the game is in progress.
        /// </summary>
        /// <returns>Returns true if the game is active.</returns>
        bool IsGameActive();
        /// <summary>
        /// Gets the maps from players who have not been found yet.
        /// </summary>
        /// <returns>Returns a dictionary with maps corresponding to players.</returns>
        Dictionary<string, Map> ViewMaps();
        /// <summary>
        /// Checks if a hider is in the specified room.
        /// </summary>
        /// <param name="seeker">The seeker who is guessing.</param>
        /// <param name="playerName">The name of the hider.</param>
        /// <param name="roomName">The name of the room.</param>
        /// <returns>Returns true if the seeker has correctly guessed the room the hider was hiding in.</returns>
        bool GuessRoom(Player seeker, string playerName, string roomName);
        /// <summary>
        /// Signals all <see cref="Player"/>s that the game has ended.
        /// </summary>
        void EndGame();
    }
}
