using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace HideNSeek.Logic
{
    /// <summary>
    /// The <see cref="Room"/> class defines a room located on a map.
    /// </summary>
    public struct Room
    {
        #region Properties
        /// <summary>
        /// The name of the room. This is the identifier of a room within a hider's hiding system.
        /// </summary>
        public string RoomName { get; private set; }
        /// <summary>
        /// This is the location relative to the floorplan file of <see cref="Map"/>.
        /// </summary>
        public Point[] Location { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Instantiate a new room for a <see cref="Map"/>.
        /// </summary>
        /// <param name="roomName">The identifier of the room.</param>
        public Room(string roomName)
        {
            RoomName = roomName;
            Location = new Point[2];
        }

        /// <summary>
        /// Instantiate a new room for a <see cref="Map"/>.
        /// </summary>
        /// <param name="roomName">The identifier of the room.</param>
        /// <param name="firstVertex">The first outer vertex of the room's location square.</param>
        /// <param name="secondVertex">The second diagonal vertex of the room's location square.</param>
        public Room(string roomName, Point firstVertex, Point secondVertex)
            : this(roomName)
        {
            Location[0] = firstVertex;
            Location[1] = secondVertex;
        }
        #endregion
    }
}
