using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon : Map
{
    private int minRoomSize = 3;
    private int maxRoomSize = 9;
    private int minCorridorLength = 3;
    private int maxCorridorLength = 7;
    private int maxStructures = 50;
    private float roomChance = 0.4f;

    /// <summary>
    /// Creates a new dungeon from several settings.
    /// </summary>
    /// <param name="sizeX">The maximum size of the map on the x-axis.</param>
    /// <param name="sizeY">The maximum size of the map on the y-axis.</param>
    /// <param name="seed">The random seed to use for the generation (set to -1 for a random seed).</param>
    /// <param name="minRoomSize">The minimum room size in tiles.</param>
    /// <param name="maxRoomSize">The maximum room size in tiles.</param>
    /// <param name="minCorridorLength">The minimum length of a corridor in tiles.</param>
    /// <param name="maxCorridorLength">The maximum length of a corridor in tiles.</param>
    /// <param name="maxStructures">The maximum amount of structures that can be on the map. Includes rooms and corridors.</param>
    /// <param name="roomChance">The chance of a room to spawn from a corridor. (must be between 0.0 and 1.0)</param>
    public Dungeon(int sizeX, int sizeY, int seed, int minRoomSize, int maxRoomSize, int minCorridorLength, int maxCorridorLength, int maxStructures, float roomChance) : base(sizeX, sizeY) {
        this.minRoomSize = minRoomSize;
        this.maxRoomSize = maxRoomSize;
        this.minCorridorLength = minCorridorLength;
        this.maxCorridorLength = maxCorridorLength;
        this.maxStructures = maxStructures;
        this.roomChance = roomChance;

        if (seed != int.MaxValue)
            Random.InitState(seed);

        // dungeon structure with rooms and tunnels
        // starting room
        int rndSizeX = Random.Range(this.minRoomSize, this.maxRoomSize + 1);
        int rndSizeY = Random.Range(this.minRoomSize, this.maxRoomSize + 1);
        int rndPosX = Size.x / 2 - rndSizeX / 2;
        int rndPosY = Size.y / 2 - rndSizeY / 2;
        Structure startRoom = new Structure(rndPosX, rndPosY, rndSizeX, rndSizeY, true);
        AddStructure(startRoom, Direction.Left, true);

        // create as many rooms as possible
        int iterations = this.maxStructures * 8;
        int structureCount = 0;
        for (int i = 0; i < iterations; i++) {
            if (CreateStructure()) {
                structureCount++;
                if (structureCount > this.maxStructures)
                    break;
            }
        }

        // if there are left over walls, try and create more rooms
        for (int i = 0; i < walls.Count; i++) {
            if (walls[i].IsRoom)
                continue;

            rndPosX = Random.Range(walls[i].Position.x, walls[i].Position.x + walls[i].Size.x);
            rndPosY = Random.Range(walls[i].Position.y, walls[i].Position.y + walls[i].Size.y);

            for (int j = 0; j < 4; j++) {
                if (CreateRoom(rndPosX, rndPosY, (Direction)j)) {
                    this[rndPosX, rndPosY] = TileType.Floor;
                    break;
                }
            }
        }

        DeleteDeadEnds();
    }

    private void DeleteDeadEnds() {
        int wallCount;
        bool foundDeadEnd = true;
        while (foundDeadEnd) {
            foundDeadEnd = false;

            for (int x = 1; x < Size.x - 1; x++) {
                for (int y = 1; y < Size.y - 1; y++) {
                    if (this[x, y] == TileType.Floor) {
                        wallCount = 0;
                        if (this[x - 1, y] == TileType.Wall)
                            wallCount++;
                        if (this[x + 1, y] == TileType.Wall)
                            wallCount++;
                        if (this[x, y - 1] == TileType.Wall)
                            wallCount++;
                        if (this[x, y + 1] == TileType.Wall)
                            wallCount++;

                        if (wallCount >= 3) {
                            foundDeadEnd = true;
                            this[x, y] = TileType.Wall;

                            for (int i = 0; i < tunnels.Count; i++) {
                                if (x == tunnels[i].Position.x && y == tunnels[i].Position.y) {
                                    tunnels[i] = new Structure {
                                        Position = new Vector2Int(tunnels[i].Position.x + 1, tunnels[i].Position.y + 1),
                                        Size = new Vector2Int(tunnels[i].Size.x - 1, tunnels[i].Size.y - 1),
                                        IsRoom = false
                                    };

                                    break;
                                }
                                if (x == tunnels[i].Position.x + tunnels[i].Size.x - 1 && y == tunnels[i].Position.y + tunnels[i].Size.y - 1) {
                                    tunnels[i] = new Structure {
                                        Position = tunnels[i].Position,
                                        Size = new Vector2Int(tunnels[i].Size.x - 1, tunnels[i].Size.y - 1),
                                        IsRoom = false
                                    };

                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private bool CreateStructure() {
        int rnd = Random.Range(0, walls.Count);
        int rndPosX = Random.Range(walls[rnd].Position.x, walls[rnd].Position.x + walls[rnd].Size.x);
        int rndPosY = Random.Range(walls[rnd].Position.y, walls[rnd].Position.y + walls[rnd].Size.y);

        for (int i = 0; i < 4; i++) {
            if (CreateStructure(rndPosX, rndPosY, (Direction)i, walls[rnd].IsRoom)) {
                walls.RemoveAt(rnd);
                return true;
            }
        }

        return false;
    }

    private bool CreateStructure(int x, int y, Direction dir, bool isRoom) {
        if (!isRoom && Random.value < roomChance) {
            if (CreateRoom(x, y, dir)) {
                this[x, y] = TileType.Floor;

                return true;
            }
        } else {
            if (CreateCorridor(x, y, dir)) {
                this[x, y] = TileType.Floor;

                return true;
            }
        }

        return false;
    }

    private bool CreateRoom(int x, int y, Direction dir) {
        Structure room = new Structure {
            Size = new Vector2Int(Random.Range(minRoomSize, maxRoomSize + 1), Random.Range(minRoomSize, maxRoomSize + 1)),
            IsRoom = true
        };

        switch (dir) {
            case Direction.Left:
                room.Position = new Vector2Int(x - room.Size.x, y - room.Size.y / 2);
                break;

            case Direction.Right:
                room.Position = new Vector2Int(x + 1, y - room.Size.y / 2);
                break;

            case Direction.Up:
                room.Position = new Vector2Int(x - room.Size.x / 2, y + 1);
                break;

            case Direction.Down:
                room.Position = new Vector2Int(x - room.Size.x / 2, y - room.Size.y);
                break;
        }

        if (IsEmptyInArea(room.Position, room.Size)) {
            AddStructure(room, dir);

            if (room.Size.x % 4 == 0 && room.Size.y % 4 == 0) {
                Structure addWall = new Structure {
                    Position = new Vector2Int(room.Position.x + room.Size.x / 4, room.Position.y + room.Size.y / 4),
                    Size = new Vector2Int(1, 1),
                    IsRoom = false
                };
                AddWallStructure(addWall);

                addWall = new Structure {
                    Position = new Vector2Int(room.Position.x + room.Size.x / 4 * 3 - 1, room.Position.y + room.Size.y / 4),
                    Size = new Vector2Int(1, 1),
                    IsRoom = false
                };
                AddWallStructure(addWall);

                addWall = new Structure {
                    Position = new Vector2Int(room.Position.x + room.Size.x / 4, room.Position.y + room.Size.y / 4 * 3 - 1),
                    Size = new Vector2Int(1, 1),
                    IsRoom = false
                };
                AddWallStructure(addWall);

                addWall = new Structure {
                    Position = new Vector2Int(room.Position.x + room.Size.x / 4 * 3 - 1, room.Position.y + room.Size.y / 4 * 3 - 1),
                    Size = new Vector2Int(1, 1),
                    IsRoom = false
                };
                AddWallStructure(addWall);
            }

            return true;
        }

        return false;
    }

    private bool CreateCorridor(int x, int y, Direction dir) {
        Structure corridor = new Structure() {
            IsRoom = false
        };

        switch (dir) {
            case Direction.Left:
                corridor.Size = new Vector2Int(Random.Range(minCorridorLength, maxCorridorLength + 1), 1);
                corridor.Position = new Vector2Int(x - corridor.Size.x, y);
                break;

            case Direction.Right:
                corridor.Size = new Vector2Int(Random.Range(minCorridorLength, maxCorridorLength + 1), 1);
                corridor.Position = new Vector2Int(x + 1, y);
                break;

            case Direction.Up:
                corridor.Size = new Vector2Int(1, Random.Range(minCorridorLength, maxCorridorLength + 1));
                corridor.Position = new Vector2Int(x, y + 1);
                break;

            case Direction.Down:
                corridor.Size = new Vector2Int(1, Random.Range(minCorridorLength, maxCorridorLength + 1));
                corridor.Position = new Vector2Int(x, y - corridor.Size.y);
                break;
        }

        if (IsEmptyInArea(corridor.Position, corridor.Size)) {
            AddStructure(corridor, dir);

            return true;
        }

        return false;
    }
}