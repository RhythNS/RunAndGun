using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator
{
    public enum Direction { Up, Down, Left, Right, None }

    public enum TileTypes { Wall, Floor, CorridorAccess, PlaceHolder, None }

    public struct Room
    {
        public Vector2Int Position { get; set; }
        public Vector2Int Size { get; set; }

        public Fast2DArray<TileTypes> layout { get; private set; }

        public Dictionary<Vector2Int, Direction> exitDirections;

        /// <summary>
        /// Creates a new room at the given position (bottom left corner) with the given layout.
        /// </summary>
        /// <param name="posX">The x-Position of the room.</param>
        /// <param name="posY">The y-Position of the room.</param>
        /// <param name="layout">The layout of the room.</param>
        public Room(int posX, int posY, Fast2DArray<TileTypes> layout) {
            Position = new Vector2Int(posX, posY);
            Size = new Vector2Int(layout.XSize, layout.YSize);
            this.layout = layout;
            exitDirections = new Dictionary<Vector2Int, Direction>();

            List<Vector2Int> checkedPositions = new List<Vector2Int>();
            for (int x = 0; x < this.layout.XSize; x++) {
                for (int y = 0; y < this.layout.YSize; y++) {
                    if (this.layout[x, y] == TileTypes.CorridorAccess && !checkedPositions.Contains(new Vector2Int(x, y))) {
                        checkedPositions.Add(new Vector2Int(x, y));

                        if (this.layout[x + 1, y] == TileTypes.CorridorAccess) {
                            if (this.layout[x, y + 1] == TileTypes.Floor) {
                                exitDirections.Add(new Vector2Int(x, y), Direction.Down);

                                int delta = 1;
                                while (this.layout[x + delta, y] == TileTypes.CorridorAccess && this.layout[x + delta + 1, y] == TileTypes.CorridorAccess) {
                                    checkedPositions.Add(new Vector2Int(x + delta, y));

                                    exitDirections.Add(new Vector2Int(x + delta, y), Direction.Down);

                                    delta++;
                                }
                            } else if (this.layout[x, y - 1] == TileTypes.Floor) {
                                exitDirections.Add(new Vector2Int(x, y), Direction.Up);

                                int delta = 1;
                                while (this.layout[x + delta, y] == TileTypes.CorridorAccess && this.layout[x + delta + 1, y] == TileTypes.CorridorAccess) {
                                    checkedPositions.Add(new Vector2Int(x + delta, y));

                                    exitDirections.Add(new Vector2Int(x + delta, y), Direction.Up);

                                    delta++;
                                }
                            }
                        } else if (this.layout[x, y + 1] == TileTypes.CorridorAccess) {
                            if (this.layout[x + 1, y] == TileTypes.Floor) {
                                exitDirections.Add(new Vector2Int(x, y), Direction.Left);

                                int delta = 1;
                                while (this.layout[x, y + delta] == TileTypes.CorridorAccess && this.layout[x, y + delta + 1] == TileTypes.CorridorAccess && this.layout[x, y + delta + 2] == TileTypes.CorridorAccess) {
                                    checkedPositions.Add(new Vector2Int(x, y + delta));

                                    exitDirections.Add(new Vector2Int(x, y + delta), Direction.Left);

                                    delta++;
                                }
                                checkedPositions.Add(new Vector2Int(x, y + delta));
                            } else if (this.layout[x - 1, y] == TileTypes.Floor) {
                                exitDirections.Add(new Vector2Int(x, y), Direction.Left);

                                int delta = 1;
                                while (this.layout[x, y + delta] == TileTypes.CorridorAccess && this.layout[x, y + delta + 1] == TileTypes.CorridorAccess && this.layout[x, y + delta + 2] == TileTypes.CorridorAccess) {
                                    checkedPositions.Add(new Vector2Int(x, y + delta));

                                    exitDirections.Add(new Vector2Int(x, y + delta), Direction.Left);

                                    delta++;
                                }
                                checkedPositions.Add(new Vector2Int(x, y + delta));
                            }
                        }
                    }
                }
            }
        }
    }

    public struct Corridor
    {
        public const int MIN_LENGTH = 10;
        public const int MAX_LENGTH = 24;

        public Vector2Int Position { get; set; }
        public Vector2Int Size { get; set; }

        public Dictionary<Vector2Int, Direction> exitDirections;

        /// <summary>
        /// Places a new corridor from the given position with the length in the given direction.
        /// </summary>
        /// <param name="posX">The x-Position where the corridor should start.</param>
        /// <param name="posY">The y-Position where the corridor should start.</param>
        /// <param name="length">The length of the corridor.</param>
        /// <param name="direction">The direction where the corridor should point to.</param>
        public Corridor(int posX, int posY, int length, Direction direction) {
            Position = Vector2Int.zero;
            Size = Vector2Int.zero;
            exitDirections = new Dictionary<Vector2Int, Direction>();

            switch (direction) {
                case Direction.Up:
                    Size = new Vector2Int(2, length);
                    Position = new Vector2Int(posX, posY);

                    for (int y = 3; y < Size.y - 6; y++) {
                        exitDirections.Add(new Vector2Int(-1, y), Direction.Left);
                        exitDirections.Add(new Vector2Int(Size.x, y), Direction.Right);
                    }

                    exitDirections.Add(new Vector2Int(0, Size.y), Direction.Up);
                    break;

                case Direction.Down:
                    Size = new Vector2Int(2, length);
                    Position = new Vector2Int(posX, posY - Size.y);

                    for (int y = 3; y < Size.y - 6; y++) {
                        exitDirections.Add(new Vector2Int(-1, y), Direction.Left);
                        exitDirections.Add(new Vector2Int(Size.x, y), Direction.Right);
                    }

                    exitDirections.Add(new Vector2Int(0, -1), Direction.Down);
                    break;

                case Direction.Left:
                    Size = new Vector2Int(length, 3);
                    Position = new Vector2Int(posX, posY);

                    for (int x = 3; x < Size.x - 5; x++) {
                        exitDirections.Add(new Vector2Int(x, Size.y), Direction.Up);
                        exitDirections.Add(new Vector2Int(x, -1), Direction.Down);
                    }

                    exitDirections.Add(new Vector2Int(-1, 0), Direction.Left);
                    break;

                case Direction.Right:
                    Size = new Vector2Int(length, 3);
                    Position = new Vector2Int(posX - Size.x, posY);

                    for (int x = 3; x < Size.x - 5; x++) {
                        exitDirections.Add(new Vector2Int(x, Size.y), Direction.Up);
                        exitDirections.Add(new Vector2Int(x, -1), Direction.Down);
                    }

                    exitDirections.Add(new Vector2Int(Size.x, 0), Direction.Right);
                    break;
            }
        }
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
            Fast2DArrayIntToTileType(ref roomLayouts, ref this.roomLayouts);

            if (seed != int.MaxValue)
                Random.InitState(seed);

            // generate starting room

            // start with corridors and then more rooms or corridors
            // from a room only corridors can be generated
            // from a corridor corridors and rooms can be generated

            // if dead ends got created, try to create more rooms
            // or delete them
        }






        public void Fast2DArrayIntToTileType(ref Fast2DArray<int>[] arrayIn, ref Fast2DArray<TileTypes>[] arrayOut) {
            for (int i = 0; i < roomLayouts.Length; i++) {
                for (int x = 0; x < arrayIn[i].XSize; x++) {
                    for (int y = 0; y < arrayIn[i].YSize; y++) {
                        arrayOut[i][x, y] = (TileTypes)arrayIn[i][x, y];
                    }
                }
            }
        }
    }
}
