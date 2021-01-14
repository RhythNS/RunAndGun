using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator
{
    public abstract class Map_OLD
    {
        public enum Direction { Left, Right, Up, Down }

        public enum TileType { Wall, Floor }

        protected List<Room_OLD> rooms;
        protected List<Structure_OLD> tunnels;
        protected List<Structure_OLD> walls;
        private List<Structure_OLD> additionalWalls;
        private TileType[][] mapTiles;

        /// <summary>
        /// The maximum size of the map.
        /// </summary>
        public Vector2Int Size { get; protected set; }

        /// <summary>
        /// Generated rooms of the map.
        /// </summary>
        public Structure_OLD[] Rooms {
            get { return rooms.ToArray(); }
        }
        /// <summary>
        /// Generated tunnels / corridors of the map.
        /// </summary>
        public Structure_OLD[] Tunnels {
            get { return tunnels.ToArray(); }
        }
        /// <summary>
        /// Generated walls of the map.
        /// </summary>
        public Structure_OLD[] Walls {
            get { return walls.ToArray(); }
        }

        /// <summary>
        /// Create an empty map.
        /// </summary>
        /// <param name="x">The maximum number of tiles on the x-axis.</param>
        /// <param name="y">The maximum number of tiles on the y-axis.</param>
        public Map_OLD(int x, int y) {
            Size = new Vector2Int(x, y);

            Reset();
        }

        /// <summary>
        /// Indexer for the map tiles.
        /// </summary>
        /// <param name="x">The x-component.</param>
        /// <param name="y">The y-component.</param>
        /// <returns>Returns the tile at the specified position.</returns>
        public TileType this[int x, int y] {
            get => mapTiles[x][y];
            protected set => mapTiles[x][y] = value;
        }

        /// <summary>
        /// Resets the map to an empty one.
        /// </summary>
        public void Reset() {
            rooms = new List<Room_OLD>();
            tunnels = new List<Structure_OLD>();
            walls = new List<Structure_OLD>();
            additionalWalls = new List<Structure_OLD>();

            mapTiles = new TileType[Size.x][];
            for (int i = 0; i < Size.x; i++) {
                mapTiles[i] = new TileType[Size.y];

                for (int j = 0; j < Size.y; j++) {
                    mapTiles[i][j] = TileType.Wall;
                }
            }
        }

        protected void AddRoom(Room_OLD room, Structure_OLD[] walls) {
            rooms.Add(room);

            this.walls.AddRange(walls);
        }

        /// <summary>
        /// Adds a new structure to the map.
        /// </summary>
        /// <param name="structure">The structure to add.</param>
        /// <param name="dir">The direction in which to add it.</param>
        /// <param name="isStartRoom">If it is the start room. Defaults to false.</param>
        protected void AddStructure(Structure_OLD structure, Direction dir) {
            for (int x = structure.Position.x; x < structure.Position.x + structure.Size.x; x++) {
                for (int y = structure.Position.y; y < structure.Position.y + structure.Size.y; y++) {
                    this[x, y] = TileType.Floor;
                }
            }

            tunnels.Add(structure);

            if (dir != Direction.Left)
                walls.Add(new Structure_OLD(
                    structure.Position.x + structure.Size.x, structure.Position.y,
                    1, structure.Size.y,
                    structure.IsRoom
                ));
            if (dir != Direction.Right)
                walls.Add(new Structure_OLD(
                    structure.Position.x - 1, structure.Position.y,
                    1, structure.Size.y,
                    structure.IsRoom
                ));
            if (dir != Direction.Up)
                walls.Add(new Structure_OLD(
                    structure.Position.x, structure.Position.y - 1,
                    structure.Size.x, 1,
                    structure.IsRoom
                ));
            if (dir != Direction.Down)
                walls.Add(new Structure_OLD(
                    structure.Position.x, structure.Position.y + structure.Size.y,
                    structure.Size.x, 1,
                    structure.IsRoom
                ));
        }

        protected void AddWallStructure(Structure_OLD structure) {
            for (int x = structure.Position.x; x < structure.Position.x + structure.Size.x; x++) {
                for (int y = structure.Position.y; y < structure.Position.y + structure.Size.y; y++) {
                    this[x, y] = TileType.Wall;
                }
            }

            additionalWalls.Add(structure);
        }

        /// <summary>
        /// Checks, if the map is empty in the specified area.
        /// </summary>
        /// <param name="position">The position of the area to check.</param>
        /// <param name="size">The size of the area to check.</param>
        /// <returns>Returns true if the map is empty in the area, false if not.</returns>
        public bool IsEmptyInArea(Vector2Int position, Vector2Int size) {
            for (int x = position.x - 1; x <= position.x + size.x; x++) {
                for (int y = position.y - 1; y <= position.y + size.y; y++) {
                    if (x >= 0 && y >= 0 && x < Size.x && y < Size.y) {
                        if (this[x, y] == TileType.Floor) {
                            return false;
                        }
                    } else {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool IsEmptyInArea(Room_OLD room) {
            for (int x = room.Position.x - 1; x <= room.Position.x + room.Size.x; x++) {
                for (int y = room.Position.y - 1; y <= room.Position.y + room.Size.y; y++) {
                    if (x >= 0 && y >= 0 && x < Size.x && y < Size.y) {
                        if (room.tiles.Get(x - room.Position.x, y - room.Position.y) == 1 && this[x, y] == TileType.Floor) {
                            return false;
                        }
                    } else {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
