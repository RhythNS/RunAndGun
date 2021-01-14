using UnityEngine;


namespace MapGenerator
{
    public class Structure_OLD
    {
        public Vector2Int Position { get; set; }
        public Vector2Int Size { get; set; }

        public bool IsRoom { get; set; }

        public Structure_OLD() { }

        public Structure_OLD(int x, int y, int sizeX, int sizeY, bool isRoom) {
            Position = new Vector2Int(x, y);
            Size = new Vector2Int(sizeX, sizeY);
            IsRoom = isRoom;
        }

        public Structure_OLD(Vector2Int position, Vector2Int size, bool isRoom) {
            Position = position;
            Size = size;
            IsRoom = isRoom;
        }
    }
}
