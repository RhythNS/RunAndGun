using System.Collections.Generic;

using UnityEngine;

namespace MapGenerator
{
    public struct Room
    {
        public Vector2Int Position { get; set; }
        public RoomType Type { get; set; }

        public Fast2DArray<TileType> Layout { get; set; }

        public Dictionary<Vector2Int, Direction> ExitDirections { get; set; }

        public List<TiledImporter.PrefabLocations> GameObjects { get; set; }

        public int TileCount { get; private set; }

        /// <summary>
        /// Creates a new room at the given position (bottom left corner) with the given layout.
        /// </summary>
        /// <param name="posX">The x-Position of the room.</param>
        /// <param name="posY">The y-Position of the room.</param>
        /// <param name="layout">The layout of the room.</param>
        public Room(int posX, int posY, Fast2DArray<TileType> layout, List<TiledImporter.PrefabLocations> gameObjects, RoomType roomType) {
            this.Position = new Vector2Int(posX, posY);
            this.Type = roomType;
            this.Layout = layout;
            this.GameObjects = gameObjects;

            this.ExitDirections = new Dictionary<Vector2Int, Direction>();

            this.TileCount = 0;

            List<Vector2Int> checkedPositions = new List<Vector2Int>();
            for (int x = 0; x < this.Layout.XSize; x++) {
                for (int y = 0; y < this.Layout.YSize; y++) {
                    if (this.Layout[x, y] == TileType.Floor) {
                        TileCount++;
                    }

                    if (this.Layout[x, y] == TileType.CorridorAccess && !checkedPositions.Contains(new Vector2Int(x, y))) {
                        checkedPositions.Add(new Vector2Int(x, y));

                        if (this.Layout[x + 1, y] == TileType.CorridorAccess) {
                            if (this.Layout[x, y + 1] == TileType.Floor) {
                                ExitDirections.Add(new Vector2Int(x, y), Direction.Down);

                                int delta = 1;
                                while (this.Layout[x + delta, y] == TileType.CorridorAccess && this.Layout[x + delta + 1, y] == TileType.CorridorAccess) {
                                    checkedPositions.Add(new Vector2Int(x + delta, y));

                                    ExitDirections.Add(new Vector2Int(x + delta, y), Direction.Down);

                                    delta++;
                                }
                            } else if (this.Layout[x, y - 1] == TileType.Floor) {
                                ExitDirections.Add(new Vector2Int(x, y), Direction.Up);

                                int delta = 1;
                                while (this.Layout[x + delta, y] == TileType.CorridorAccess && this.Layout[x + delta + 1, y] == TileType.CorridorAccess) {
                                    checkedPositions.Add(new Vector2Int(x + delta, y));

                                    ExitDirections.Add(new Vector2Int(x + delta, y), Direction.Up);

                                    delta++;
                                }
                            }
                        } else if (this.Layout[x, y + 1] == TileType.CorridorAccess) {
                            if (this.Layout[x + 1, y] == TileType.Floor) {
                                ExitDirections.Add(new Vector2Int(x, y), Direction.Left);

                                int delta = 1;
                                while (this.Layout[x, y + delta] == TileType.CorridorAccess && this.Layout[x, y + delta + 1] == TileType.CorridorAccess && this.Layout[x, y + delta + 2] == TileType.CorridorAccess) {
                                    checkedPositions.Add(new Vector2Int(x, y + delta));

                                    ExitDirections.Add(new Vector2Int(x, y + delta), Direction.Left);

                                    delta++;
                                }
                                checkedPositions.Add(new Vector2Int(x, y + delta));
                            } else if (this.Layout[x - 1, y] == TileType.Floor) {
                                ExitDirections.Add(new Vector2Int(x, y), Direction.Right);

                                int delta = 1;
                                while (this.Layout[x, y + delta] == TileType.CorridorAccess && this.Layout[x, y + delta + 1] == TileType.CorridorAccess && this.Layout[x, y + delta + 2] == TileType.CorridorAccess) {
                                    checkedPositions.Add(new Vector2Int(x, y + delta));

                                    ExitDirections.Add(new Vector2Int(x, y + delta), Direction.Right);

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
            get => Layout[x, y];
        }

        public List<Vector2Int> GetWalkableTiles() {
            List<Vector2Int> walkableTiles = new List<Vector2Int>();

            for (int x = 0; x < Layout.XSize; x++) {
                for (int y = 0; y < Layout.YSize; y++) {
                    if (Layout[x, y] == TileType.Floor)
                        walkableTiles.Add(new Vector2Int(x, y));
                }
            }

            return walkableTiles;
        }
    }
}
