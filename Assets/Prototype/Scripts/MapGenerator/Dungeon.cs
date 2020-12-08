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
    
    public Dungeon(int sizeX, int sizeY, int seed, int minRoomSize, int maxRoomSize, int minCorridorLength, int maxCorridorLength, int maxStructures, float roomChance) : base(sizeX, sizeY) {
        this.minRoomSize = minRoomSize;
        this.maxRoomSize = maxRoomSize;
        this.minCorridorLength = minCorridorLength;
        this.maxCorridorLength = maxCorridorLength;
        this.maxStructures = maxStructures;
        this.roomChance = roomChance;

        if (seed != -1)
            Random.InitState(seed);

        //// 100% random
        //for (int i = 0; i < sizeX; i++) {
        //    for (int j = 0; j < sizeY; j++) {
        //        float rnd = Random.value;
        //        if (rnd < 0.1f) {
        //            this[x, y] = Tile.Floor;
        //        }
        //    }
        //}

        //// random using mapper
        //Vector2Int currPos = new Vector2Int((int)((float)SizeX / (float)2), (int)((float)SizeY / (float)2));
        //int maxSteps = 50;
        //for (int i = 0; i < maxSteps; i++) {
        //    int rnd = Random.Range(0, 4);

        //    switch (rnd) {
        //        case 0:
        //            if (currPos.x < SizeX - 2)
        //                currPos.x++;
        //            break;

        //        case 1:
        //            if (currPos.x > 1)
        //                currPos.x--;
        //            break;

        //        case 2:
        //            if (currPos.y < SizeY - 2)
        //                currPos.y++;
        //            break;

        //        case 3:
        //            if (currPos.y > 1)
        //                currPos.y--;
        //            break;
        //    }

        //    this[currPos.x, currPos.y] = Tile.Floor;
        //}

        // dungeon structure with rooms and tunnels
        // starting room
        int rndSizeX = Random.Range(this.minRoomSize, this.maxRoomSize + 1);
        int rndSizeY = Random.Range(this.minRoomSize, this.maxRoomSize + 1);
        int rndPosX = SizeX / 2 - rndSizeX / 2;
        int rndPosY = SizeY / 2 - rndSizeY / 2;
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
            } else {
                Debug.Log("Cannot create more structures!");
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
                    this[rndPosX, rndPosY] = Tile.Floor;
                    break;
                }
            }
        }

        // delete dead-ends
        bool foundDeadEnd = true;
        while (foundDeadEnd) {
            foundDeadEnd = false;

            for (int x = 1; x < SizeX - 1; x++) {
                for (int y = 1; y < SizeY - 1; y++) {
                    if (this[x, y] == Tile.Floor) {
                        int wallCount = 0;
                        if (this[x - 1, y] == Tile.Wall)
                            wallCount++;
                        if (this[x + 1, y] == Tile.Wall)
                            wallCount++;
                        if (this[x, y - 1] == Tile.Wall)
                            wallCount++;
                        if (this[x, y + 1] == Tile.Wall)
                            wallCount++;

                        if (wallCount >= 3) {
                            foundDeadEnd = true;
                            this[x, y] = Tile.Wall;
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
                this[x, y] = Tile.Floor;

                return true;
            }
        } else {
            if (CreateCorridor(x, y, dir)) {
                this[x, y] = Tile.Floor;

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
                corridor.Position = new Vector2Int(x, y);
                break;

            case Direction.Up:
                corridor.Size = new Vector2Int(1, Random.Range(minCorridorLength, maxCorridorLength + 1));
                corridor.Position = new Vector2Int(x, y);
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
