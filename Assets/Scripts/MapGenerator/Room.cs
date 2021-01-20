using System.Collections.Generic;

using UnityEngine;

namespace MapGenerator
{
    public struct Room
    {
        public Vector2Int Position { get; set; }
        public Vector2Int Size { get; set; }

        public RoomType Type { get; private set; }

        public Fast2DArray<TileType> layout { get; private set; }

        public Dictionary<Vector2Int, Direction> exitDirections { get; private set; }

        public List<TiledImporter.PrefabContainer> gameObjects { get; private set; }

        /// <summary>
        /// Creates a new room at the given position (bottom left corner) with the given layout.
        /// </summary>
        /// <param name="posX">The x-Position of the room.</param>
        /// <param name="posY">The y-Position of the room.</param>
        /// <param name="layout">The layout of the room.</param>
        public Room(int posX, int posY, Fast2DArray<TileType> layout, List<TiledImporter.PrefabContainer> gameObjects, RoomType roomType) {
            Position = new Vector2Int(posX, posY);
            Size = new Vector2Int(layout.XSize, layout.YSize);
            this.layout = layout;

            exitDirections = new Dictionary<Vector2Int, Direction>();

            this.gameObjects = gameObjects;
            this.Type = roomType;

            List<Vector2Int> checkedPositions = new List<Vector2Int>();
            for (int x = 0; x < this.layout.XSize; x++) {
                for (int y = 0; y < this.layout.YSize; y++) {
                    if (this.layout[x, y] == TileType.CorridorAccess && !checkedPositions.Contains(new Vector2Int(x, y))) {
                        checkedPositions.Add(new Vector2Int(x, y));

                        if (this.layout[x + 1, y] == TileType.CorridorAccess) {
                            if (this.layout[x, y + 1] == TileType.Floor) {
                                exitDirections.Add(new Vector2Int(x, y), Direction.Down);

                                int delta = 1;
                                while (this.layout[x + delta, y] == TileType.CorridorAccess && this.layout[x + delta + 1, y] == TileType.CorridorAccess) {
                                    checkedPositions.Add(new Vector2Int(x + delta, y));

                                    exitDirections.Add(new Vector2Int(x + delta, y), Direction.Down);

                                    delta++;
                                }
                            } else if (this.layout[x, y - 1] == TileType.Floor) {
                                exitDirections.Add(new Vector2Int(x, y), Direction.Up);

                                int delta = 1;
                                while (this.layout[x + delta, y] == TileType.CorridorAccess && this.layout[x + delta + 1, y] == TileType.CorridorAccess) {
                                    checkedPositions.Add(new Vector2Int(x + delta, y));

                                    exitDirections.Add(new Vector2Int(x + delta, y), Direction.Up);

                                    delta++;
                                }
                            }
                        } else if (this.layout[x, y + 1] == TileType.CorridorAccess) {
                            if (this.layout[x + 1, y] == TileType.Floor) {
                                exitDirections.Add(new Vector2Int(x, y), Direction.Left);

                                int delta = 1;
                                while (this.layout[x, y + delta] == TileType.CorridorAccess && this.layout[x, y + delta + 1] == TileType.CorridorAccess && this.layout[x, y + delta + 2] == TileType.CorridorAccess) {
                                    checkedPositions.Add(new Vector2Int(x, y + delta));

                                    exitDirections.Add(new Vector2Int(x, y + delta), Direction.Left);

                                    delta++;
                                }
                                checkedPositions.Add(new Vector2Int(x, y + delta));
                            } else if (this.layout[x - 1, y] == TileType.Floor) {
                                exitDirections.Add(new Vector2Int(x, y), Direction.Right);

                                int delta = 1;
                                while (this.layout[x, y + delta] == TileType.CorridorAccess && this.layout[x, y + delta + 1] == TileType.CorridorAccess && this.layout[x, y + delta + 2] == TileType.CorridorAccess) {
                                    checkedPositions.Add(new Vector2Int(x, y + delta));

                                    exitDirections.Add(new Vector2Int(x, y + delta), Direction.Right);

                                    delta++;
                                }
                                checkedPositions.Add(new Vector2Int(x, y + delta));
                            }
                        }
                    }
                }
            }
        }

        public TileType this[int x, int y] {
            get => layout[x, y];
        }
    }
}
