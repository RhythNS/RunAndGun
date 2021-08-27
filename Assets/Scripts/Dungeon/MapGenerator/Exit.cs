using UnityEngine;

namespace MapGenerator
{
    /// <summary>
    /// Structure for an exit position of a room / corridor.
    /// </summary>
    public struct Exit
    {
        public Vector2Int Position { get; set; }

        public Direction Direction { get; set; }

        /// <summary>
        /// If this exit is from a room.
        /// </summary>
        public bool IsRoomExit { get; set; }

        /// <summary>
        /// If this exit goes into a room.
        /// </summary>
        public bool IntoRoom { get; set; }
    }
}
