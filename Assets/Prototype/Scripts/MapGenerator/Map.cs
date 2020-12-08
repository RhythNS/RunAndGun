using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Structure
{
    public Vector2Int Position { get; set; }
    public Vector2Int Size { get; set; }

    public bool IsRoom { get; set; }

    public Structure(int x, int y, int sizeX, int sizeY, bool isRoom) {
        Position = new Vector2Int(x, y);
        Size = new Vector2Int(sizeX, sizeY);
        IsRoom = isRoom;
    }
}

public abstract class Map
{
    public enum Direction { Left, Right, Up, Down }

    public enum Tile { Wall, Floor }

    /// -------------- Debug getters, remove if not wanted ---------------------
    public List<Structure> Rooms => rooms;
    public List<Structure> Tunnels => tunnels;
    public List<Structure> Walls => walls;
    /// -------------- Debug getters, remove if not wanted ---------------------

    private List<Structure> rooms = new List<Structure>();
    private List<Structure> tunnels = new List<Structure>();
    protected List<Structure> walls = new List<Structure>();
    private Tile[][] mapTiles;

    public int SizeX { get; protected set; }
    public int SizeY { get; protected set; }

    /// <summary>
    /// Create a new map.
    /// </summary>
    /// <param name="x">The maximum number of tiles on the x-axis.</param>
    /// <param name="y">The maximum number of tiles on the y-axis.</param>
    public Map(int x, int y) {
        SizeX = x;
        SizeY = y;

        mapTiles = new Tile[SizeX][];
        for (int i = 0; i < SizeX; i++) {
            mapTiles[i] = new Tile[SizeY];

            for (int j = 0; j < SizeY; j++) {
                mapTiles[i][j] = Tile.Wall;
            }
        }
    }

    /// <summary>
    /// Indexer for the map tiles.
    /// </summary>
    /// <param name="x">The x-component.</param>
    /// <param name="y">The y-component.</param>
    /// <returns>Returns the tile at the specified position.</returns>
    public Tile this[int x, int y] {
        get => mapTiles[x][y];
        protected set => mapTiles[x][y] = value;
    }

    public void Reset() {
        rooms = new List<Structure>();
        tunnels = new List<Structure>();
        walls = new List<Structure>();

        mapTiles = new Tile[SizeX][];
        for (int i = 0; i < SizeX; i++) {
            mapTiles[i] = new Tile[SizeY];

            for (int j = 0; j < SizeY; j++) {
                mapTiles[i][j] = Tile.Wall;
            }
        }
    }

    /// <summary>
    /// Adds a new structure to the map.
    /// </summary>
    /// <param name="structure">The structure to add.</param>
    /// <param name="dir">The direction in which to add it.</param>
    /// <param name="isStartRoom">If it is the start room. Defaults to false.</param>
    protected void AddStructure(Structure structure, Direction dir, bool isStartRoom = false) {
        for (int x = structure.Position.x; x < structure.Position.x + structure.Size.x; x++) {
            for (int y = structure.Position.y; y < structure.Position.y + structure.Size.y; y++) {
                this[x, y] = Tile.Floor;
            }
        }

        if (structure.IsRoom) {
            rooms.Add(structure);

            if (dir != Direction.Left || isStartRoom)
                walls.Add(new Structure(
                    structure.Position.x + structure.Size.x, structure.Position.y + 1,
                    1, structure.Size.y - 2,
                    structure.IsRoom
                ));
            if (dir != Direction.Right || isStartRoom)
                walls.Add(new Structure(
                    structure.Position.x - 1, structure.Position.y + 1,
                    1, structure.Size.y - 2,
                    structure.IsRoom
                ));
            if (dir != Direction.Up || isStartRoom)
                walls.Add(new Structure(
                    structure.Position.x + 1, structure.Position.y - 1,
                    structure.Size.x - 2, 1,
                    structure.IsRoom
                ));
            if (dir != Direction.Down || isStartRoom)
                walls.Add(new Structure(
                    structure.Position.x + 1, structure.Position.y + structure.Size.y,
                    structure.Size.x - 2, 1,
                    structure.IsRoom
                ));
        } else {
            tunnels.Add(structure);

            if (dir != Direction.Left || isStartRoom)
                walls.Add(new Structure(
                    structure.Position.x + structure.Size.x, structure.Position.y,
                    1, structure.Size.y,
                    structure.IsRoom
                ));
            if (dir != Direction.Right || isStartRoom)
                walls.Add(new Structure(
                    structure.Position.x - 1, structure.Position.y,
                    1, structure.Size.y,
                    structure.IsRoom
                ));
            if (dir != Direction.Up || isStartRoom)
                walls.Add(new Structure(
                    structure.Position.x, structure.Position.y - 1,
                    structure.Size.x, 1,
                    structure.IsRoom
                ));
            if (dir != Direction.Down || isStartRoom)
                walls.Add(new Structure(
                    structure.Position.x, structure.Position.y + structure.Size.y,
                    structure.Size.x, 1,
                    structure.IsRoom
                ));
        }
    }

    /// <summary>
    /// Checks, if the map is empty in the specified area.
    /// </summary>
    /// <param name="position">The position of the area to check.</param>
    /// <param name="size">The size of the area to check.</param>
    /// <returns>Returns true if the map is empty in the area, false if not.</returns>
    protected bool IsEmptyInArea(Vector2Int position, Vector2Int size) {
        for (int x = position.x - 1; x <= position.x + size.x; x++) {
            for (int y = position.y - 1; y <= position.y + size.y; y++) {
                if (x >= 0 && y >= 0 && x < SizeX && y < SizeY) {
                    if (this[x, y] == Tile.Floor) {
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
