using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator
{
    public enum Direction { North, South, West, East, None }

    public enum TileTypes { Wall, Floor, None }

    public struct Room
    {
        public Vector2Int Position { get; set; }
        public Vector2Int Size { get; set; }

        public Fast2DArray<TileTypes> layout { get; private set; }

        public Room(int posX, int posY, Fast2DArray<TileTypes> intLayout) {
            Position = new Vector2Int(posX, posY);
            Size = new Vector2Int(intLayout.XSize, intLayout.YSize);
            layout = intLayout;
        }
    }

    public struct Corridor
    {
        public Vector2Int Position { get; set; }
        public Vector2Int Size { get; set; }
    }

    class Dungeon
    {
        private Vector2Int size;
        private Fast2DArray<TileTypes>[] roomLayouts;

        private List<Room> rooms;
        private List<Corridor> corridors;

        public Dungeon(int sizeX, int sizeY, Fast2DArray<int>[] roomLayouts, int maxStructures, int seed) {
            this.size = new Vector2Int(sizeX, sizeY);
            this.roomLayouts = new Fast2DArray<TileTypes>[roomLayouts.Length];
            for (int i = 0; i < roomLayouts.Length; i++) {
                for (int x = 0; x < roomLayouts[i].XSize; x++) {
                    for (int y = 0; y < roomLayouts[i].YSize; y++) {
                        this.roomLayouts[i][x, y] = (TileTypes)roomLayouts[i][x, y];
                    }
                }
            }

            if (seed != int.MaxValue)
                Random.InitState(seed);

            // generate starting room

            // start with corridors and then more rooms or corridors
            // from a room only corridors can be generated
            // from a corridor corridors and rooms can be generated

            // if dead ends got created, try to create more rooms
            // or delete them
        }
    }
}
