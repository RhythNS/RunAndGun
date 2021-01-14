using UnityEngine;

namespace MapGenerator
{
    public class Room_OLD : Structure_OLD
    {
        public Fast2DArray<int> tiles;

        public Room_OLD() { }

        public Room_OLD(Fast2DArray<int> tiles, int x, int y) {
            this.tiles = tiles;

            Position = new Vector2Int(x, y);
            Size = new Vector2Int(tiles.XSize, tiles.YSize);
            IsRoom = true;
        }
    }
}
