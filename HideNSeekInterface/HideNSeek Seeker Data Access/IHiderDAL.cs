using System.Collections.Generic;

namespace HideNSeek.DAL.Seeker
{
    /// <summary>
    /// An interface that allows communication to a hiding system.
    /// </summary>
    public interface IHiderDAL
    {
        /// <summary>
        /// Gets the room names of each room on the hiders floorplan.
        /// </summary>
        /// <returns>A list of room names.</returns>
        IEnumerable<string> GetRooms();
        /// <summary>
        /// Signal to the hiding system that a new game has started.
        /// </summary>
        void StartHiding();
        /// <summary>
        /// Gets the hiders current trail.
        /// </summary>
        /// <returns>The path the hider has taken through the house.</returns>
        string GetCurrentPath();
        /// <summary>
        /// Gets the hider's current position.
        /// </summary>
        /// <returns>The name of the room the hider is currently in.</returns>
        string GetCurrentRoom();
    }
}
