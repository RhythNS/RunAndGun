using UnityEngine;

namespace MapGenerator
{
    public struct Exit
    {
        public Vector2Int Position { get; set; }
        public Direction Direction { get; set; }
        public bool IsRoomExit { get; set; }
        public bool IntoRoom { get; set; }
    }
}
