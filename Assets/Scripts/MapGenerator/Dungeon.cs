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






        /// <summary>
        /// Returns the TileType of a given tile
        /// </summary>
        /// <param name="x">x-value of the tile to check</param>
        /// <param name="y">y-value of the tile to check</param>
        /// <returns>Returns the TileType of the given tile</returns>
        private TileType GetTileType(int x, int y) {
            if (x < 0 || y < 0 || x >= mapLayout.XSize || y >= mapLayout.YSize)
                return TileType.Wall;

            return mapLayout[x, y];
        }

        /// <summary>
        /// Returns the positions of the neighbours that are walls of a given tile
        /// </summary>
        /// <param name="x">x-value of the tile to check</param>
        /// <param name="y">y-value of the tile to check</param>
        /// <returns>Returns the positions of the neighbours that are walls of a given tile</returns>
        public Vector2Int[] GetNeighbours(int x, int y) {
            List<Vector2Int> neighbours = new List<Vector2Int>();

            if (GetTileType(x - 1, y) == TileType.Floor)
                neighbours.Add(new Vector2Int(x - 1, y));
            if (GetTileType(x, y - 1) == TileType.Floor)
                neighbours.Add(new Vector2Int(x, y - 1));
            if (GetTileType(x + 1, y) == TileType.Floor)
                neighbours.Add(new Vector2Int(x + 1, y));
            if (GetTileType(x, y + 1) == TileType.Floor)
                neighbours.Add(new Vector2Int(x, y + 1));

            return neighbours.ToArray();
        }

        /// <summary>
        /// Gets the pathfinding cost of a given tile
        /// </summary>
        /// <param name="tile"></param>
        /// <returns>Returns the pathfinding cost of a given tile</returns>
        public float GetCost(Vector2Int tile) {
            return 5f;
            // calculate costs here
            //return Map[tile.x, tile.y].specialType == SpecialTypes.None || Map[tile.x, tile.y].specialType == SpecialTypes.FloorToWater ? 5f : 20f;
        }

        /// <summary>
        /// Tries to find a new path through the cave using A*
        /// </summary>
        /// <param name="start">Where the pathfinding should start</param>
        /// <param name="destination">The destination of the path</param>
        /// <returns>All tile positions of the path found</returns>
        public Vector3[] FindPath(Vector2Int start, Vector2Int destination) {
            Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
            Dictionary<Vector2Int, float> costSoFar = new Dictionary<Vector2Int, float>();

            Vector2Int current;

            PriorityQueue<Vector2Int> border = new PriorityQueue<Vector2Int>();
            border.Enqueue(start, 0f);

            cameFrom.Add(start, start);
            costSoFar.Add(start, 0f);

            while (border.Count > 0) {
                current = border.Dequeue();

                if (current.Equals(destination)) break;

                foreach (Vector2Int neighbour in GetNeighbours(current.x, current.y)) {
                    float newCost = costSoFar[current] + GetCost(neighbour);

                    if (!costSoFar.ContainsKey(neighbour) || newCost < costSoFar[neighbour]) {
                        if (costSoFar.ContainsKey(neighbour)) {
                            costSoFar.Remove(neighbour);
                            cameFrom.Remove(neighbour);
                        }

                        costSoFar.Add(neighbour, newCost);
                        cameFrom.Add(neighbour, current);
                        float priority = newCost + Mathf.Abs(neighbour.x - destination.x) + Mathf.Abs(neighbour.y - destination.y);
                        border.Enqueue(neighbour, priority);
                    }
                }
            }

            List<Vector3> path = new List<Vector3>();
            current = destination;

            while (!current.Equals(start)) {
                if (!cameFrom.ContainsKey(current)) {
                    //Debug.Log("cameFrom does not contain current"); // no path found
                    return new Vector3[0];
                }

                path.Add(new Vector3(current.x, current.y, 0f));
                current = cameFrom[current];
            }

            path.Reverse();

            if (path.Count > 1)
                path.RemoveAt(0);

            return path.ToArray();
        }

    }

    /// <summary>
    /// PriorityQueue for use within the A* path finding
    /// https://gist.github.com/keithcollins/307c3335308fea62db2731265ab44c06
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PriorityQueue<T>
    {
        // From Red Blob: I'm using an unsorted array for this example, but ideally this
        // would be a binary heap. Find a binary heap class:
        // * https://bitbucket.org/BlueRaja/high-speed-priority-queue-for-c/wiki/Home
        // * http://visualstudiomagazine.com/articles/2012/11/01/priority-queues-with-c.aspx
        // * http://xfleury.github.io/graphsearch.html
        // * http://stackoverflow.com/questions/102398/priority-queue-in-net

        private List<KeyValuePair<T, float>> elements = new List<KeyValuePair<T, float>>();

        public int Count {
            get { return elements.Count; }
        }

        public void Enqueue(T item, float priority) {
            elements.Add(new KeyValuePair<T, float>(item, priority));
        }

        // Returns the Location that has the lowest priority
        public T Dequeue() {
            int bestIndex = 0;

            for (int i = 0; i < elements.Count; i++) {
                if (elements[i].Value < elements[bestIndex].Value) {
                    bestIndex = i;
                }
            }

            T bestItem = elements[bestIndex].Key;
            elements.RemoveAt(bestIndex);
            return bestItem;
        }
    }
}
