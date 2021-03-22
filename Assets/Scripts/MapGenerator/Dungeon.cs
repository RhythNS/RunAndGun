using System.Collections.Generic;

using UnityEngine;

namespace MapGenerator
{
    public class Dungeon
    {
        private const int ITERATIONS = 128;

        /// <summary>
        /// The maximum size of the Dungeon.
        /// </summary>
        public Vector2Int Size { get; private set; }
        private readonly Fast2DArray<TileType> mapLayout;

        private readonly Fast2DArray<TileType>[] roomLayouts;
        private readonly List<TiledImporter.PrefabLocations>[] roomGameObjects;
        private readonly RoomType[] roomTypes;

        private readonly List<Room> rooms = new List<Room>();
        private readonly List<Corridor> corridors = new List<Corridor>();

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

        /// <summary>
        /// Gets the TileType at the specified location.
        /// </summary>
        /// <param name="x">The x-position of the tile.</param>
        /// <param name="y">The y-position of the tile.</param>
        /// <returns>Returns the TileType.</returns>
        public TileType this[int x, int y] {
            get => mapLayout[x, y];
        }

        /// <summary>
        /// Creates and generates a new Dungeon.
        /// </summary>
        /// <param name="roomLayouts">The layouts for the rooms the Dungeon should contain.</param>
        /// <param name="roomGameObjects">The list of GameObjects that should be spawned for the rooms.</param>
        /// <param name="roomTypes">The list of GameObjects that should be spawned.</param>
        /// <param name="config">The data to use when generating the Dungeon.</param>
        public Dungeon(Fast2DArray<int>[] roomLayouts, List<TiledImporter.PrefabLocations>[] roomGameObjects, RoomType[] roomTypes, DungeonConfig config) {
            Size = new Vector2Int(config.sizeX, config.sizeY);
            mapLayout = new Fast2DArray<TileType>(Size.x, Size.y);
            this.roomLayouts = new Fast2DArray<TileType>[roomLayouts.Length];
            Fast2DArrayIntToTileType(ref roomLayouts, ref this.roomLayouts);
            this.roomGameObjects = roomGameObjects;
            this.roomTypes = roomTypes;

            List<Exit> exits = new List<Exit>();

            if (config.seed != int.MaxValue)
                Random.InitState(config.seed);

            // generate starting room
            Room startRoom = new Room((int)(Size.x / 2f - this.roomLayouts[0].XSize / 2f), (int)(Size.y / 2f - this.roomLayouts[0].YSize / 2f), this.roomLayouts[0], this.roomGameObjects[0], this.roomTypes[0]);
            AddRoom(startRoom);

            // generate the initial map structure with corridors and rooms
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

            // try and generate as many rooms as possible
            int iters = 0;
            while (rooms.Count < config.minRooms && iters < ITERATIONS) {
                foreach (var exit in exits) {
                    if (exit.IntoRoom) {
                        GenerateRoom(exit);
                    }
                }

                iters++;
            }

            // generate shop and bossroom
            if (config.generateShopRoom)
                GenerateShopRoom();
            GenerateBossRoom();

            // delete dead ends from the map
            for (int i = 0; i < 3; i++) {
                DeleteDeadEnds();
            }
        }

        private void GenerateBossRoom() {
            List<Exit> exits = new List<Exit>();
            GetAllExits(ref exits);

            bool roomCreated = false;
            foreach (var exit in exits) {
                if (exit.IntoRoom && GenerateRoom(exit, true)) {
                    roomCreated = true;
                    break;
                }
            }
            while (!roomCreated) {
                GetAllExits(ref exits);

                for (int i = 0; i < exits.Count; i++) {
                    if (exits[i].Direction != Direction.Down && exits[i].Direction != Direction.Left && GenerateCorridor(exits[i])) {
                        foreach (var exit in exits) {
                            if (exit.IntoRoom && GenerateRoom(exit, true)) {
                                roomCreated = true;
                                break;
                            }
                        }

                        if (roomCreated)
                            break;
                    }
                }
            }
        }

        private void GenerateShopRoom() {
            List<Exit> exits = new List<Exit>();
            GetAllExits(ref exits);

            bool roomCreated = false;
            foreach (var exit in exits) {
                if (exit.IntoRoom && GenerateRoom(exit, false, true)) {
                    roomCreated = true;
                    break;
                }
            }
            while (!roomCreated) {
                GetAllExits(ref exits);

                for (int i = 0; i < exits.Count; i++) {
                    if (exits[i].Direction != Direction.Down && exits[i].Direction != Direction.Left && GenerateCorridor(exits[i])) {
                        foreach (var exit in exits) {
                            if (exit.IntoRoom && GenerateRoom(exit, false, true)) {
                                roomCreated = true;
                                break;
                            }
                        }

                        if (roomCreated)
                            break;
                    }
                }
            }
        }

        private void DeleteDeadEnds() {
            for (int x = 0; x < Size.x; x++) {
                mapLayout[x, 0] = TileType.Wall;
                mapLayout[x, Size.y - 1] = TileType.Wall;
            }
            for (int y = 0; y < Size.y; y++) {
                mapLayout[0, y] = TileType.Wall;
                mapLayout[Size.x - 1, y] = TileType.Wall;
            }

            // if dead ends got created, delete them
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

        private bool GenerateRoom(Exit exit, bool isBossRoom = false, bool isShopRoom = false) {
            int rndRoom = Random.Range(3, roomLayouts.Length);
            if (isBossRoom)
                rndRoom = 1;
            if (isShopRoom)
                rndRoom = 2;
            Room room = new Room(0, 0, roomLayouts[rndRoom], roomGameObjects[rndRoom], roomTypes[rndRoom]);

            foreach (var exitDir in room.ExitDirections) {
                if (!mapLayout.InBounds(exit.Position.x - exitDir.Key.x, exit.Position.y - exitDir.Key.y))
                    break;

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

                        return true;
                    }
                }
            }

            return false;
        }

        private bool GenerateCorridor(Exit exit) {
            int length = Random.Range(Corridor.MIN_LENGTH, Corridor.MAX_LENGTH + 1);
            Corridor corridor = new Corridor(exit.Position.x, exit.Position.y, length, exit.Direction);

            if (IsEmptyInArea(corridor, exit.Direction)) {
                AddCorridor(corridor);

                return true;
            }

            return false;
        }

        private bool IsEmptyInArea(Room room, Direction direction) {
            switch (direction) {
                case Direction.Up:
                    for (int x = -4; x < room.Layout.XSize + 4; x++) {
                        for (int y = 0; y < room.Layout.YSize + 4; y++) {
                            if (mapLayout[room.Position.x + x, room.Position.y + y] != TileType.Wall) {
                                return false;
                            }
                        }
                    }
                    break;

                case Direction.Down:
                    for (int x = -4; x < room.Layout.XSize + 4; x++) {
                        for (int y = -4; y < room.Layout.YSize; y++) {
                            if (mapLayout[room.Position.x + x, room.Position.y + y] != TileType.Wall) {
                                return false;
                            }
                        }
                    }
                    break;

                case Direction.Left:
                    for (int x = -4; x < room.Layout.XSize; x++) {
                        for (int y = -4; y < room.Layout.YSize + 4; y++) {
                            if (mapLayout[room.Position.x + x, room.Position.y + y] != TileType.Wall) {
                                return false;
                            }
                        }
                    }
                    break;

                case Direction.Right:
                    for (int x = 0; x < room.Layout.XSize + 4; x++) {
                        for (int y = -4; y < room.Layout.YSize + 4; y++) {
                            if (mapLayout[room.Position.x + x, room.Position.y + y] != TileType.Wall) {
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

            if (room.Position.x + room.Layout.XSize >= this.Size.x) {
                int newX = room.Position.x + room.Layout.XSize + 1;
                Debug.Log("Resizing to x: " + newX);
                mapLayout.Resize(newX, this.Size.y);

                this.Size = new Vector2Int(newX, this.Size.y);
            }
            if (room.Position.y + room.Layout.YSize >= this.Size.y) {
                int newY = room.Position.y + room.Layout.YSize + 1;
                Debug.Log("Resizing to y: " + newY);
                mapLayout.Resize(this.Size.x, newY);

                this.Size = new Vector2Int(this.Size.x, newY);
            }

            for (int x = 0; x < room.Layout.XSize; x++) {
                for (int y = 0; y < room.Layout.YSize; y++) {
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
                foreach (var exit in room.ExitDirections) {
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

        public DoorLocations[] GetDoorsOfRoom(int roomId) {
            List<DoorLocations> doorLocations = new List<DoorLocations>();

            foreach (var exitDir in rooms[roomId].ExitDirections) {
                if (mapLayout[rooms[roomId].Position.x + exitDir.Key.x, rooms[roomId].Position.y + exitDir.Key.y] == TileType.Floor) {
                    if (exitDir.Value == Direction.Left || exitDir.Value == Direction.Right) {
                        if (mapLayout[rooms[roomId].Position.x + exitDir.Key.x, rooms[roomId].Position.y + exitDir.Key.y - 1] != TileType.Floor)
                            doorLocations.Add(new DoorLocations { IsLeftRight = true, Position = rooms[roomId].Position + exitDir.Key });
                    } else {
                        if (mapLayout[rooms[roomId].Position.x + exitDir.Key.x - 1, rooms[roomId].Position.y + exitDir.Key.y] != TileType.Floor && mapLayout[rooms[roomId].Position.x + exitDir.Key.x - 2, rooms[roomId].Position.y + exitDir.Key.y] != TileType.Floor)
                            doorLocations.Add(new DoorLocations { IsLeftRight = false, Position = rooms[roomId].Position + exitDir.Key });
                    }
                }
            }

            return doorLocations.ToArray();
        }

        public List<Vector2Int> GetWalkableTiles(int roomId) {
            List<Vector2Int> walkableTiles = rooms[roomId].GetWalkableTiles();

            for (int i = 0; i < walkableTiles.Count; i++) {
                walkableTiles[i] += rooms[roomId].Position;
            }

            return walkableTiles;
        }

        #region Pathfinding
        /// <summary>
        /// Returns the TileType of a given tile
        /// </summary>
        /// <param name="x">x-value of the tile to check</param>
        /// <param name="y">y-value of the tile to check</param>
        /// <returns>The TileType of the given tile</returns>
        private TileType GetTileType(int x, int y) {
            if (x < 0 || y < 0 || x >= mapLayout.XSize || y >= mapLayout.YSize)
                return TileType.Wall;

            return mapLayout[x, y];
        }

        /// <summary>
        /// Returns the positions of the neighbours that are walkable tiles of a given tile.
        /// </summary>
        /// <param name="x">x-value of the tile to check</param>
        /// <param name="y">y-value of the tile to check</param>
        /// <returns>The positions of the neighbours that are walkable tiles of a given tile.</returns>
        private List<Vector2Int> GetNeighbours(int x, int y) {
            List<Vector2Int> neighbours = new List<Vector2Int>();

            if (GetTileType(x - 1, y) == TileType.Floor) // left
                neighbours.Add(new Vector2Int(x - 1, y));
            if (GetTileType(x, y - 1) == TileType.Floor) // down
                neighbours.Add(new Vector2Int(x, y - 1));
            if (GetTileType(x + 1, y) == TileType.Floor) // right
                neighbours.Add(new Vector2Int(x + 1, y));
            if (GetTileType(x, y + 1) == TileType.Floor) // up
                neighbours.Add(new Vector2Int(x, y + 1));

            if (GetTileType(x - 1, y - 1) == TileType.Floor) // left down
                neighbours.Add(new Vector2Int(x - 1, y - 1));
            if (GetTileType(x + 1, y - 1) == TileType.Floor) // right down
                neighbours.Add(new Vector2Int(x + 1, y - 1));
            if (GetTileType(x - 1, y + 1) == TileType.Floor) // left up
                neighbours.Add(new Vector2Int(x - 1, y + 1));
            if (GetTileType(x + 1, y + 1) == TileType.Floor) // right up
                neighbours.Add(new Vector2Int(x + 1, y + 1));

            return neighbours;
        }

        /// <summary>
        /// Gets the pathfinding cost of a given tile
        /// </summary>
        /// <param name="tile"></param>
        /// <returns>Returns the pathfinding cost of a given tile</returns>
        private float GetCost(/*Vector2Int tile*/) {
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
        public List<Vector2> TryFindPath(Vector2Int start, Vector2Int destination) {
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
                    float newCost = costSoFar[current] + GetCost(/*neighbour*/);

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

            List<Vector2> path = new List<Vector2>();
            current = destination;

            while (!current.Equals(start)) {
                if (!cameFrom.ContainsKey(current)) {
                    //Debug.Log("cameFrom does not contain current"); // no path found
                    return path;
                }

                //path.Add(new Vector2(current.x, current.y));
                path.Add(DungeonCreator.Instance.TilePositionToWorldPositionMiddle(current.x, current.y));
                current = cameFrom[current];
            }

            path.Reverse();

            if (path.Count > 1)
                path.RemoveAt(0);

            return path;
        }

        /// <summary>
        /// PriorityQueue for use within the A* path finding
        /// https://gist.github.com/keithcollins/307c3335308fea62db2731265ab44c06
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class PriorityQueue<T>
        {
            // From Red Blob: I'm using an unsorted array for this example, but ideally this
            // would be a binary heap. Find a binary heap class:
            // * https://bitbucket.org/BlueRaja/high-speed-priority-queue-for-c/wiki/Home
            // * http://visualstudiomagazine.com/articles/2012/11/01/priority-queues-with-c.aspx
            // * http://xfleury.github.io/graphsearch.html
            // * http://stackoverflow.com/questions/102398/priority-queue-in-net

            private readonly List<KeyValuePair<T, float>> elements = new List<KeyValuePair<T, float>>();

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
        #endregion
    }

    public struct DoorLocations
    {
        public Vector2Int Position { get; set; }
        public bool IsLeftRight { get; set; }
    }
}
