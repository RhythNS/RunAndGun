using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon
{
    [System.Serializable]
    public class DungeonTileset
    {
        public GameObject tileStraight;
        public GameObject tileCurve;
        public GameObject tileDeadend;
        public GameObject tileJunction;
        public GameObject tileTJunction;
    }

    public enum TileTypes { Empty, Tile }
    private Vector2Int maxSize = new Vector2Int(20, 20);

    private TileTypes[,] map;
    public TileTypes[,] Map {
        get {
            return map;
        }
    }

    private List<Section> sections = new List<Section>();
    private List<Section> walls = new List<Section>();

    [SerializeField, Range(1, 500)]
    private int maxSections = 50;
    [SerializeField, Range(0f, 1f)]
    private float roomChance = 0.4f;

    public Dungeon(Vector2Int maxSize) {
        this.maxSize = maxSize;
        map = new TileTypes[this.maxSize.x, this.maxSize.y];

        GenerateDungeon();
    }

    public void Reset() {
        map = new TileTypes[this.maxSize.x, this.maxSize.y];
    }

    private void GenerateDungeon() {
        // entrance
        //Vector2Int currPos = new Vector2Int(this.maxSize.x / 2, 1);
        //map[currPos.x, currPos.y] = TileTypes.Tile;

        // 100% random
        //for (int x = 0; x < maxSize.x; x++) {
        //    for (int y = 0; y < maxSize.y; y++) {
        //        for (int z = 0; z < maxSize.z; z++) {
        //            float rnd = Random.value;
        //            if (rnd < 0.1f) {
        //                map[x, y, z] = TileTypes.Tile;
        //            }
        //            Debug.Log(rnd);
        //        }
        //    }
        //}

        // 100% random using mapper
        //int maxSteps = 50;
        //for (int i = 0; i < maxSteps; i++) {
        //    int rnd = Random.Range(0, 4);

        //    switch (rnd) {
        //        case 0: // forward
        //            if (currPos.z < maxSize.z - 2)
        //                currPos.z += 1;
        //            break;
        //        case 1: // backward
        //            if (currPos.z > 1)
        //                currPos.z += -1;
        //            break;
        //        case 2: // left
        //            if (currPos.x > 1)
        //                currPos.x += -1;
        //            break;
        //        case 3: // right
        //            if (currPos.x < maxSize.y - 2)
        //                currPos.x += 1;
        //            break;
        //    }

        //    map[currPos.x, currPos.y, currPos.z] = TileTypes.Tile;
        //}

        // random non-overlapping dungeon structure

        Vector2Int middle = new Vector2Int(map.GetLength(0) / 2, map.GetLength(1) / 2);

        Vector2Int rndSize = new Vector2Int(Random.Range(3, 9), Random.Range(3, 9));
        Vector2Int rndPos = new Vector2Int(middle.x - rndSize.x / 2, middle.y - rndSize.y / 2);
        Section startRoom = new Section(rndPos, rndSize, SectionType.Room);
        sections.Add(startRoom);
        MapSection(startRoom, Direction.Backward, true);

        for (int i = 1; i < maxSections; i++) {
            if (!CreateSection()) {
                Debug.Log("Cannot create more sections!");
            }
        }

        for (int i = 0; i < walls.Count; i++) {
            if (walls[i].Type == SectionType.Corridor) {

                Vector2Int pos = new Vector2Int(
                    Random.Range(walls[i].Position.x, walls[i].Position.x + walls[i].Size.x - 1),
                    Random.Range(walls[i].Position.y, walls[i].Position.y + walls[i].Size.y - 1)
                );

                for (int j = 0; j < 4; j++) {
                    if (CreateRoom(pos, (Direction)j)) {
                        map[pos.x, pos.y] = TileTypes.Tile;
                        break;
                    }
                }
            }
        }
    }

    private bool CreateSection() {
        int rnd = Random.Range(0, walls.Count);
        Vector2Int rndPos = new Vector2Int(
            Random.Range(walls[rnd].Position.x, walls[rnd].Position.x + walls[rnd].Size.x - 1),
            Random.Range(walls[rnd].Position.y, walls[rnd].Position.y + walls[rnd].Size.y - 1)
        );

        for (int j = 0; j < 4; j++) {
            if (CreateSection(rndPos, (Direction)j, walls[rnd].Type)) {
                walls.RemoveAt(rnd);
                return true;
            }
        }

        return false;
    }

    private bool CreateSection(Vector2Int position, Direction dir, SectionType type) {
        if (type == SectionType.Corridor && Random.value < roomChance) {
            if (CreateRoom(position, dir)) {
                map[position.x, position.y] = TileTypes.Tile;

                return true;
            }
        } else {
            if (CreateCorridor(position, dir)) {
                map[position.x, position.y] = TileTypes.Tile;

                return true;
            }
        }

        return false;
    }

    private bool CreateRoom(Vector2Int position, Direction dir) {
        Section room = new Section {
            Size = new Vector2Int(Random.Range(3, 9), Random.Range(3, 9))
        };
        room.Type = SectionType.Room;

        switch (dir) {
            case Direction.Forward:
                room.Position = new Vector2Int(position.x - room.Size.x / 2, position.y - room.Size.y);
                break;

            case Direction.Backward:
                room.Position = new Vector2Int(position.x - room.Size.x / 2, position.y + 1);
                break;

            case Direction.Right:
                room.Position = new Vector2Int(position.x + 1, position.y - room.Size.y / 2);
                break;

            case Direction.Left:
                room.Position = new Vector2Int(position.x - room.Size.x, position.y - room.Size.y / 2);
                break;
        }

        if (IsMapEmptyInArea(room.Position, room.Size)) {
            sections.Add(room);
            MapSection(room, dir);

            return true;
        }

        return false;
    }

    private bool CreateCorridor(Vector2Int position, Direction dir) {
        Section corridor = new Section();
        corridor.Type = SectionType.Corridor;

        switch (dir) {
            case Direction.Forward:
                corridor.Size = new Vector2Int(1, Random.Range(3, 7));
                corridor.Position = new Vector2Int(position.x, position.y);
                break;

            case Direction.Backward:
                corridor.Size = new Vector2Int(1, Random.Range(3, 7));
                corridor.Position = new Vector2Int(position.x, position.y - corridor.Size.y);
                break;

            case Direction.Right:
                corridor.Size = new Vector2Int(Random.Range(3, 7), 1);
                corridor.Position = new Vector2Int(position.x, position.y);
                break;

            case Direction.Left:
                corridor.Size = new Vector2Int(Random.Range(3, 7), 1);
                corridor.Position = new Vector2Int(position.x - corridor.Size.x, position.y);
                break;
        }

        if (IsMapEmptyInArea(corridor.Position, corridor.Size)) {
            sections.Add(corridor);
            MapSection(corridor, dir);

            return true;
        }

        return false;
    }

    private void MapSection(Section section, Direction dir, bool startRoom = false) {
        for (int x = 0; x < section.Size.x; x++) {
            for (int y = 0; y < section.Size.y; y++) {
                if (section.Position.x + x < map.GetLength(0) && section.Position.y + y < map.GetLength(1)) {
                    map[section.Position.x + x, section.Position.y + y] = TileTypes.Tile;
                }
            }
        }

        bool isWall = false;
        if (section.Size.x == 1 || section.Size.y == 1)
            isWall = true;

        if (!isWall) {
            // wall south
            if (dir != Direction.Forward || startRoom)
                walls.Add(new Section(
                    new Vector2Int(section.Position.x + 1, section.Position.y - 1),
                    new Vector2Int(section.Size.x - 2, 1),
                    section.Type
                ));
            // wall north
            if (dir != Direction.Backward || startRoom)
                walls.Add(new Section(
                    new Vector2Int(section.Position.x + 1, section.Position.y + section.Size.y),
                    new Vector2Int(section.Size.x - 2, 1),
                    section.Type
                ));
            // wall west
            if (dir != Direction.Right || startRoom)
                walls.Add(new Section(
                    new Vector2Int(section.Position.x - 1, section.Position.y + 1),
                    new Vector2Int(1, section.Size.y - 2),
                    section.Type
                ));
            // wall east
            if (dir != Direction.Left || startRoom)
                walls.Add(new Section(
                    new Vector2Int(section.Position.x + section.Size.x, section.Position.y + 1),
                    new Vector2Int(1, section.Size.y - 2),
                    section.Type
                ));
        } else {
            // wall south
            if (dir != Direction.Forward || startRoom)
                walls.Add(new Section(
                    new Vector2Int(section.Position.x, section.Position.y - 1),
                    new Vector2Int(section.Size.x, 1),
                    section.Type
                ));
            // wall north
            if (dir != Direction.Backward || startRoom)
                walls.Add(new Section(
                    new Vector2Int(section.Position.x, section.Position.y + section.Size.y),
                    new Vector2Int(section.Size.x, 1),
                    section.Type
                ));
            // wall west
            if (dir != Direction.Right || startRoom)
                walls.Add(new Section(
                    new Vector2Int(section.Position.x - 1, section.Position.y),
                    new Vector2Int(1, section.Size.y),
                    section.Type
                ));
            // wall east
            if (dir != Direction.Left || startRoom)
                walls.Add(new Section(
                    new Vector2Int(section.Position.x + section.Size.x, section.Position.y),
                    new Vector2Int(1, section.Size.y),
                    section.Type
                ));
        }
    }

    private bool IsMapEmptyInArea(Vector2Int position, Vector2Int size) {
        for (int x = position.x - 1; x < position.x + size.x + 1; x++) {
            for (int y = position.y - 1; y < position.y + size.y + 1; y++) {
                if (x >= 0 && y >= 0 && x < map.GetLength(0) && y < map.GetLength(1)) {
                    if (map[x, y] == TileTypes.Tile) {
                        return false;
                    }
                } else {
                    return false;
                }
            }
        }

        return true;
    }

    private struct Section
    {
        public Vector2Int Position { get; set; }
        public Vector2Int Size { get; set; }

        public SectionType Type { get; set; }

        public Section(Vector2Int position, Vector2Int size, SectionType type) {
            this.Position = position;
            this.Size = size;
            this.Type = type;
        }
    }

    private enum SectionType { Room, Corridor }

    private enum Direction { Right, Left, Forward, Backward }
}
