using System.Collections.Generic;
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
        /// <param name="username">The identifier of the user for this lobby.</param>
        /// <returns>Returns a player object for the new player.</returns>
        Task<Player> ConnectAsync(string address, string username);
        /// <summary>
        /// Disconnects a player from the lobby.
        /// </summary>
        /// <param name="player">The player to disconnect.</param>
        Task DisconnectAsync(Player player);
        /// <summary>
        /// Gets the remaining time for the hiders to keep hiding.
        /// </summary>
        /// <returns>Returns the time in seconds.</returns>
        Task<int> GetRemainingHidingTimeAsync();
        /// <summary>
        /// Returns whether the game is in progress.
        /// </summary>
        /// <returns>Returns true if the game is active.</returns>
        bool IsGameActive();
        /// <summary>
        /// Gets the maps from players who have not been found yet.
        /// </summary>
        /// <returns>Returns a dictionary with maps corresponding to players.</returns>
        Task<Dictionary<string, Map>> ViewMapsAsync();
        /// <summary>
        /// Checks if a hider is in the specified room.
        /// </summary>
        /// <param name="seeker">The seeker who is guessing.</param>
        /// <param name="playerName">The name of the hider.</param>
        /// <param name="roomName">The name of the room.</param>
        /// <returns>Returns true if the seeker has correctly guessed the room the hider was hiding in.</returns>
        Task<bool> GuessRoomAsync(Player seeker, string playerName, string roomName);
        /// <summary>
        /// Gets the position of a <see cref="Hider"/>.
        /// </summary>
        /// <param name="hiderUsername">The identifier of the hider.</param>
        /// <returns>The <see cref="Room"/> a <see cref="Hider"/> is currently in.</returns>
        Task<Room> GetHiderLocation(string hiderUsername);
    }
}
