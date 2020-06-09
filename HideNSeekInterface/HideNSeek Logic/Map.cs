using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HideNSeek.Logic
{
    /// <summary>
    /// The <see cref="Map"/> class defines a map associated with a <see cref="Hider"/>.
    /// </summary>
    public class Map
    {
        #region Fields
        private string _floorplanFile;
        private Dictionary<string, Room> _rooms;
        #endregion

        #region Properties
        /// <summary>
        /// The file location of the floorplan.
        /// </summary>
        public string FloorplanFile
        {
            get { return _floorplanFile; }
            set
            {
                _rooms.Clear();
                _floorplanFile = value;
            }
        }
        /// <summary>
        /// The rooms assigned to the map.
        /// </summary>
        public IEnumerable<Room> Rooms {
            get
            {
                return _rooms.Values;
            }
            private set
            {
                _rooms = value.ToDictionary(room => room.RoomName);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Instantiate a new map for a <see cref="Player"/>.
        /// </summary>
        public Map()
        {
            _rooms = new Dictionary<string, Room>();
        }

        /// <summary>
        /// Instantiate a new map for a <see cref="Player"/>.
        /// </summary>
        /// <param name="floorplanFilePath">The location of the floorplan on local storage.</param>
        public Map(string floorplanFilePath) : this()
        {
            this._floorplanFile = floorplanFilePath;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Add a new <see cref="Room"/> to the the <see cref="Map"/>.
        /// </summary>
        /// <param name="room">The room data.</param>
        public void AddRoom(Room room)
        {
            _rooms.Add(room.RoomName, room);
        }

        /// <summary>
        /// Update an existing <see cref="Room"/> on the <see cref="Map"/>.
        /// </summary>
        /// <param name="room">The room data.</param>
        /// <exception cref="KeyNotFoundException">The room doesn't exist on the map yet.</exception>
        public void UpdateRoom(Room room)
        {
            _rooms[room.RoomName] = room;
        }

        /// <summary>
        /// Removes a <see cref="Room"/> from the <see cref="Map"/>.
        /// </summary>
        /// <param name="room">The room data.</param>
        /// <exception cref="KeyNotFoundException">The room doesn't exist on the map yet.</exception>
        public void RemoveRoom(string roomName)
        {
            _rooms.Remove(roomName);
        }
        /// <summary>
        /// Removes a <see cref="Room"/> from the <see cref="Map"/>.
        /// </summary>
        /// <param name="room">The room data.</param>
        /// <exception cref="KeyNotFoundException">The room doesn't exist on the map yet.</exception>
        public void RemoveRoom(Room room)
        {
            _rooms.Remove(room.RoomName);
        }

        /// <summary>
        /// Clears the <see cref="Map"/>.
        /// </summary>
        public void ResetMap()
        {
            _rooms.Clear();
        }
        #endregion
    }
}
