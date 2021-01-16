using System.Collections.Generic;

using UnityEngine;

namespace MapGenerator
{
    public class Dungeon
    {
        private const int ITERATIONS = 128;
        private const float ROOM_CHANCE = 0.4f;

        public Vector2Int Size { get; private set; }
        private Fast2DArray<TileType> mapLayout;

        /// <summary>
        /// layouts for the rooms
        /// </summary>
        private Fast2DArray<TileType>[] roomLayouts;
        private List<GameObject>[] roomGameObjects;

        private List<Room> rooms = new List<Room>();
        private List<Corridor> corridors = new List<Corridor>();

        /// <summary>
        /// Returns an array with all the generated rooms.
        /// </summary>
        public Room[] Rooms {
            get => rooms.ToArray();
        }

        /// <summary>
        /// Returns an array with all the generated corridors.
        /// </summary>
        public Corridor[] Corridors {
            get => corridors.ToArray();
        }

        public Dungeon(int sizeX, int sizeY, Fast2DArray<int>[] roomLayouts, List<GameObject>[] roomGameObjects, int maxStructures, int seed) {
            this.Size = new Vector2Int(sizeX, sizeY);
            mapLayout = new Fast2DArray<TileType>(Size.x, Size.y);
            this.roomLayouts = new Fast2DArray<TileType>[roomLayouts.Length];
            Fast2DArrayIntToTileType(ref roomLayouts, ref this.roomLayouts);
            this.roomGameObjects = roomGameObjects;

            List<Exit> exits = new List<Exit>();

            if (seed != int.MaxValue)
                Random.InitState(seed);

            // generate starting room
            Room startRoom = new Room((int)(Size.x / 2f - this.roomLayouts[0].XSize / 2f), (int)(Size.y / 2f - this.roomLayouts[0].YSize / 2f), this.roomLayouts[0], this.roomGameObjects[0]);
            AddRoom(startRoom);

            for (int i = 0; i < ITERATIONS; i++) {
                GetAllExits(ref exits);

                int rndExit = Random.Range(0, exits.Count);
                if (exits[rndExit].IsRoomExit) {
                    GenerateCorridor(exits[rndExit]);
                } else {
                    if (exits[rndExit].IntoRoom) {
                        GenerateRoom(exits[rndExit]);
                    } else {
                        GenerateCorridor(exits[rndExit]);
                    }
                }
            }

            int iters = 0;
            while (rooms.Count < 10 && iters < ITERATIONS) {
                foreach (var exit in exits) {
                    if (exit.IntoRoom) {
                        GenerateRoom(exit);
                    }
                }

                iters++;
            }

            for (int i = 0; i < 5; i++) {
                DeleteDeadEnds();
            }
        }

        public TileType this[int x, int y] {
            get => mapLayout[x, y];
        }


        private void DeleteDeadEnds() {
            // if dead ends got created, try to create more rooms
            // or delete them
            foreach (var corridor in corridors) {
                if (corridor.Size.x == 2) {
                    // up down
                    int delta = 0;
                    while (mapLayout[corridor.Position.x, corridor.Position.y - 1 + delta] != TileType.Floor
                        && mapLayout[corridor.Position.x + 1, corridor.Position.y - 1 + delta] != TileType.Floor
                        && mapLayout[corridor.Position.x - 1, corridor.Position.y + delta] != TileType.Floor
                        && mapLayout[corridor.Position.x + 2, corridor.Position.y + delta] != TileType.Floor
                        ) {

                        if (mapLayout.InBounds(corridor.Position.x, corridor.Position.y + delta)) {
                            mapLayout[corridor.Position.x, corridor.Position.y + delta] = TileType.Wall;
                            mapLayout[corridor.Position.x + 1, corridor.Position.y + delta] = TileType.Wall;
                        } else {
                            break;
                        }

                        delta++;
                    }
                    delta = corridor.Size.y - 1;
                    while (mapLayout[corridor.Position.x, corridor.Position.y + delta + 1] != TileType.Floor
                        && mapLayout[corridor.Position.x + 1, corridor.Position.y + delta + 1] != TileType.Floor
                        && mapLayout[corridor.Position.x - 1, corridor.Position.y + delta] != TileType.Floor
                        && mapLayout[corridor.Position.x + 2, corridor.Position.y + delta] != TileType.Floor
                        ) {

                        if (mapLayout.InBounds(corridor.Position.x, corridor.Position.y + delta)) {
                            mapLayout[corridor.Position.x, corridor.Position.y + delta] = TileType.Wall;
                            mapLayout[corridor.Position.x + 1, corridor.Position.y + delta] = TileType.Wall;
                        } else {
                            break;
                        }

                        delta--;
                    }
                } else if (corridor.Size.y == 3) {
                    // left right
                    int delta = 0;
                    while (mapLayout[corridor.Position.x - 1 + delta, corridor.Position.y] != TileType.Floor
                        && mapLayout[corridor.Position.x - 1 + delta, corridor.Position.y + 1] != TileType.Floor
                        && mapLayout[corridor.Position.x - 1 + delta, corridor.Position.y + 2] != TileType.Floor
                        && mapLayout[corridor.Position.x + delta, corridor.Position.y - 1] != TileType.Floor
                        && mapLayout[corridor.Position.x + delta, corridor.Position.y + 3] != TileType.Floor
                        ) {

                        if (mapLayout.InBounds(corridor.Position.x + delta, corridor.Position.y)) {
                            mapLayout[corridor.Position.x + delta, corridor.Position.y] = TileType.Wall;
                            mapLayout[corridor.Position.x + delta, corridor.Position.y + 1] = TileType.Wall;
                            mapLayout[corridor.Position.x + delta, corridor.Position.y + 2] = TileType.Wall;
                        } else {
                            break;
                        }

                        delta++;
                    }

                    delta = corridor.Size.x - 1;
                    while (mapLayout[corridor.Position.x + 1 + delta, corridor.Position.y] != TileType.Floor
                        && mapLayout[corridor.Position.x + 1 + delta, corridor.Position.y + 1] != TileType.Floor
                        && mapLayout[corridor.Position.x + 1 + delta, corridor.Position.y + 2] != TileType.Floor
                        && mapLayout[corridor.Position.x + delta, corridor.Position.y - 1] != TileType.Floor
                        && mapLayout[corridor.Position.x + delta, corridor.Position.y + 3] != TileType.Floor
                        ) {

                        if (mapLayout.InBounds(corridor.Position.x + delta, corridor.Position.y)) {
                            mapLayout[corridor.Position.x + delta, corridor.Position.y] = TileType.Wall;
                            mapLayout[corridor.Position.x + delta, corridor.Position.y + 1] = TileType.Wall;
                            mapLayout[corridor.Position.x + delta, corridor.Position.y + 2] = TileType.Wall;
                        } else {
                            break;
                        }

                        delta--;
                    }
                }
            }
        }

        private void GenerateRoom(Exit exit) {
            int rndRoom = Random.Range(0, roomLayouts.Length);
            Room room = new Room(0, 0, roomLayouts[rndRoom], roomGameObjects[rndRoom]);

            foreach (var exitDir in room.exitDirections) {
                if ((exitDir.Value == Direction.Up && exit.Direction == Direction.Down)
                    || (exitDir.Value == Direction.Down && exit.Direction == Direction.Up)
                    || (exitDir.Value == Direction.Left && exit.Direction == Direction.Right)
                    || (exitDir.Value == Direction.Right && exit.Direction == Direction.Left)
                    ) {
                    room.Position = exit.Position - exitDir.Key;

                    if (IsEmptyInArea(room, exit.Direction)) {
                        if (exit.Direction == Direction.Down || exit.Direction == Direction.Up) {
                            mapLayout[exit.Position.x, exit.Position.y] = TileType.Floor;
                            mapLayout[exit.Position.x + 1, exit.Position.y] = TileType.Floor;
                        } else {
                            mapLayout[exit.Position.x, exit.Position.y] = TileType.Floor;
                            mapLayout[exit.Position.x, exit.Position.y + 1] = TileType.Floor;
                            mapLayout[exit.Position.x, exit.Position.y + 2] = TileType.Floor;
                        }

                        AddRoom(room);
                    }

                    break;
                }
            }



        }

        private void GenerateCorridor(Exit exit) {
            int length = Random.Range(Corridor.MIN_LENGTH, Corridor.MAX_LENGTH + 1);
            Corridor corridor = new Corridor(exit.Position.x, exit.Position.y, length, exit.Direction);

            if (IsEmptyInArea(corridor, exit.Direction)) {
                AddCorridor(corridor);
            }
        }

        private bool IsEmptyInArea(Room room, Direction direction) {
            switch (direction) {
                case Direction.Up:
                    for (int x = -2; x < room.Size.x + 2; x++) {
                        for (int y = 0; y < room.Size.y + 2; y++) {
                            if ((room[x, y] != TileType.Wall && room[x, y] != TileType.CorridorAccess) && mapLayout[room.Position.x + x, room.Position.y + y] != TileType.Wall) {
                                return false;
                            }
                        }
                    }
                    break;

                case Direction.Down:
                    for (int x = -2; x < room.Size.x + 2; x++) {
                        for (int y = -2; y < room.Size.y; y++) {
                            if ((room[x, y] != TileType.Wall && room[x, y] != TileType.CorridorAccess) && mapLayout[room.Position.x + x, room.Position.y + y] != TileType.Wall) {
                                return false;
                            }
                        }
                    }
                    break;

                case Direction.Left:
                    for (int x = -2; x < room.Size.x; x++) {
                        for (int y = -2; y < room.Size.y + 2; y++) {
                            if ((room[x, y] != TileType.Wall && room[x, y] != TileType.CorridorAccess) && mapLayout[room.Position.x + x, room.Position.y + y] != TileType.Wall) {
                                return false;
                            }
                        }
                    }
                    break;

                case Direction.Right:
                    for (int x = 0; x < room.Size.x + 2; x++) {
                        for (int y = -2; y < room.Size.y + 2; y++) {
                            if ((room[x, y] != TileType.Wall && room[x, y] != TileType.CorridorAccess) && mapLayout[room.Position.x + x, room.Position.y + y] != TileType.Wall) {
                                return false;
                            }
                        }
                    }
                    break;
            }

            return true;
        }

        private bool IsEmptyInArea(Corridor corridor, Direction direction) {
            switch (direction) {
                case Direction.Up:
                    for (int x = -2; x < corridor.Size.x + 2; x++) {
                        for (int y = 0; y < corridor.Size.y + 2; y++) {
                            if (mapLayout[corridor.Position.x + x, corridor.Position.y + y] != TileType.Wall) {
                                return false;
                            }
                        }
                    }
                    break;

                case Direction.Down:
                    for (int x = -2; x < corridor.Size.x + 2; x++) {
                        for (int y = -2; y < corridor.Size.y; y++) {
                            if (mapLayout[corridor.Position.x + x, corridor.Position.y + y] != TileType.Wall) {
                                return false;
                            }
                        }
                    }
                    break;

                case Direction.Left:
                    for (int x = -2; x < corridor.Size.x; x++) {
                        for (int y = -2; y < corridor.Size.y + 3; y++) {
                            if (mapLayout[corridor.Position.x + x, corridor.Position.y + y] != TileType.Wall) {
                                return false;
                            }
                        }
                    }
                    break;

                case Direction.Right:
                    for (int x = 0; x < corridor.Size.x + 2; x++) {
                        for (int y = -2; y < corridor.Size.y + 2; y++) {
                            if (mapLayout[corridor.Position.x + x, corridor.Position.y + y] != TileType.Wall) {
                                return false;
                            }
                        }
                    }
                    break;
            }

            return true;
        }

        private void AddRoom(Room room) {
            rooms.Add(room);

            for (int x = 0; x < room.Size.x; x++) {
                for (int y = 0; y < room.Size.y; y++) {
                    if (room[x, y] != TileType.Wall && room[x, y] != TileType.CorridorAccess) {
                        if (mapLayout.InBounds(room.Position.x + x, room.Position.y + y))
                            mapLayout[room.Position.x + x, room.Position.y + y] = room[x, y];
                    }
                }
            }
        }

        private void AddCorridor(Corridor corridor) {
            corridors.Add(corridor);

            for (int x = 0; x < corridor.Size.x; x++) {
                for (int y = 0; y < corridor.Size.y; y++) {
                    if (mapLayout.InBounds(corridor.Position.x + x, corridor.Position.y + y))
                        mapLayout[corridor.Position.x + x, corridor.Position.y + y] = TileType.Floor;
                }
            }
        }

        private void Fast2DArrayIntToTileType(ref Fast2DArray<int>[] arrayIn, ref Fast2DArray<TileType>[] arrayOut) {
            for (int i = 0; i < roomLayouts.Length; i++) {
                arrayOut[i] = new Fast2DArray<TileType>(arrayIn[i].XSize, arrayIn[i].YSize);
                for (int x = 0; x < arrayIn[i].XSize; x++) {
                    for (int y = 0; y < arrayIn[i].YSize; y++) {
                        arrayOut[i][x, y] = (TileType)arrayIn[i][x, y];
                    }
                }
            }
        }

        private void GetAllExits(ref List<Exit> exits) {
            exits.Clear();

            foreach (Room room in rooms) {
                foreach (var exit in room.exitDirections) {
                    exits.Add(new Exit {
                        Position = new Vector2Int(room.Position.x + exit.Key.x, room.Position.y + exit.Key.y),
                        Direction = exit.Value,
                        IsRoomExit = true,
                        IntoRoom = false
                    });
                }
            }

            foreach (Corridor corridor in corridors) {
                foreach (var exit in corridor.exitDirections) {
                    Exit e = new Exit {
                        Position = new Vector2Int(corridor.Position.x + exit.Key.x, corridor.Position.y + exit.Key.y),
                        Direction = exit.Value,
                        IsRoomExit = false
                    };

                    if (corridor.Size.x == 2) {
                        // up down
                        if (exit.Key.x == -1 || exit.Key.x == 2) {
                            e.IntoRoom = false;
                        } else {
                            e.IntoRoom = true;
                        }
                    } else if (corridor.Size.y == 3) {
                        // left right
                        if (exit.Key.y == -1 || exit.Key.y == 3) {
                            e.IntoRoom = false;
                        } else {
                            e.IntoRoom = true;
                        }
                    }

                    exits.Add(e);
                }
            }
        }
    }
}
