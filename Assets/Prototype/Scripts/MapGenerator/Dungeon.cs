using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon : Map
{
    private const int MIN_ROOM_SIZE = 3;
    private const int MAX_ROOM_SIZE = 9;
    private const int MIN_CORRIDOR_LENGTH = 3;
    private const int MAX_CORRIDOR_LENGTH = 7;
    private const int MAX_STRUCTURES = 50;
    private const float ROOM_CHANCE = 0.5f;
    
    public Dungeon(int x, int y, int seed) : base(x, y) {
        SizeX = x;
        SizeY = y;

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
        //Vector2Int currPos = new Vector2Int(sizeX / 2, sizeY / 2);
        //int maxSteps = 50;
        //for (int i = 0; i < maxSteps; i++) {
        //    int rnd = Random.Range(0, 4);

        //    switch (rnd) {
        //        case 0:
        //            if (currPos.x < sizeX - 2)
        //                currPos.x++;
        //            break;

        //        case 1:
        //            if (currPos.x > 1)
        //                currPos.x--;
        //            break;

        //        case 2:
        //            if (currPos.y < sizeY - 2)
        //                currPos.y++;
        //            break;

        //        case 3:
        //            if (currPos.y > 1)
        //                currPos.y--;
        //            break;
        //    }

        //    this[x, y] = Tile.Floor;
        //}

        // dungeon structure with rooms and tunnels
        // starting room
        int rndSizeX = Random.Range(MIN_ROOM_SIZE, MAX_ROOM_SIZE);
        int rndSizeY = Random.Range(MIN_ROOM_SIZE, MAX_ROOM_SIZE);
        int rndPosX = SizeX / 2 - rndSizeX / 2;
        int rndPosY = SizeY / 2 - rndSizeY / 2;
        Structure startRoom = new Structure(rndSizeX, rndSizeY, rndPosX, rndPosY, true);
        AddStructure(startRoom, Direction.Left, true);

        // create as many rooms as possible
        for (int i = 0; i < MAX_STRUCTURES; i++) {
            if (!CreateStructure())
                Debug.Log("Cannot create more structures!");
        }

        //// if there are left over walls, try and create more rooms
        //for (int i = 0; i < walls.Count; i++) {
        //    if (!walls[i].IsRoom)
        //        continue;

        //    rndPosX = Random.Range(walls[i].Position.x, walls[i].Position.x + walls[i].Size.x - 1);
        //    rndPosY = Random.Range(walls[i].Position.y, walls[i].Position.y + walls[i].Size.y - 1);

        //    for (int j = 0; j < 4; j++) {
        //        if (CreateRoom(rndPosX, rndPosY, (Direction)j)) {
        //            this[x, y] = Tile.Floor;
        //            break;
        //        }
        //    }
        //}
    }

    private bool CreateStructure() {
        int rnd = Random.Range(0, walls.Count);
        int rndPosX = Random.Range(walls[rnd].Position.x, walls[rnd].Position.x + walls[rnd].Size.x - 1);
        int rndPosY = Random.Range(walls[rnd].Position.y, walls[rnd].Position.y + walls[rnd].Size.y - 1);

        for (int i = 0; i < 4; i++) {
            if (CreateStructure(rndPosX, rndPosY, (Direction)i, walls[rnd].IsRoom)) {
                walls.RemoveAt(rnd);
                return true;
            }
        }

        return false;
    }

    private bool CreateStructure(int x, int y, Direction dir, bool isRoom) {
        if (isRoom && Random.value < ROOM_CHANCE) {
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
            Size = new Vector2Int(Random.Range(MIN_ROOM_SIZE, MAX_ROOM_SIZE), Random.Range(MIN_ROOM_SIZE, MAX_ROOM_SIZE)),
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
                corridor.Size = new Vector2Int(Random.Range(MIN_CORRIDOR_LENGTH, MAX_CORRIDOR_LENGTH), 1);
                corridor.Position = new Vector2Int(x - corridor.Size.x, y);
                break;

            case Direction.Right:
                corridor.Size = new Vector2Int(Random.Range(MIN_CORRIDOR_LENGTH, MAX_CORRIDOR_LENGTH), 1);
                corridor.Position = new Vector2Int(x, y);
                break;

            case Direction.Up:
                corridor.Size = new Vector2Int(1, Random.Range(MIN_CORRIDOR_LENGTH, MAX_CORRIDOR_LENGTH));
                corridor.Position = new Vector2Int(x, y);
                break;

            case Direction.Down:
                corridor.Size = new Vector2Int(1, Random.Range(MIN_CORRIDOR_LENGTH, MAX_CORRIDOR_LENGTH));
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
