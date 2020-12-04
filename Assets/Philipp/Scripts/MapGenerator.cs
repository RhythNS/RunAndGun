using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapGenerator
{
    public static Map GenerateMap(int seed) {
        if (seed != -1)
            Random.InitState(seed);

        Tree tree = new Tree();

        Map map = tree.GenerateNewMap();

        return map;
    }

    public enum MapTile { Wall, Floor }

    public class Structure
    {
        public Vector2Int Pos { get; set; }
        public Vector2Int Size { get; set; }
    }

    public class Leaf
    {
        private static readonly int minLeafSize = 8;
        private readonly int depth = 0;
        public bool SplitTried { get; private set; } = false;

        public Vector2Int Pos { get; private set; }
        public Vector2Int Size { get; private set; }

        public Leaf LeftChild { get; private set; }
        public Leaf RightChild { get; private set; }

        public Structure Room { get; private set; } = null;

        public Leaf(int x, int y, int width, int height, int depth) {
            this.Pos = new Vector2Int(x, y);
            this.Size = new Vector2Int(width, height);
            this.depth = depth;
        }

        public bool Split() {
            SplitTried = true;

            // if already split, return false
            if (LeftChild != null || RightChild != null)
                return false;

            // random chance or too deep, return false
            if (depth > 2 && Random.value > 0.3f || depth > 3)
                return false;

            // split horizontally or vertically
            bool splitWidth;
            if (Size.x > Size.y)
                splitWidth = true;
            else if (Size.y > Size.x)
                splitWidth = false;
            else
                splitWidth = Random.value > 0.5f;

            // determine longest side and get its length
            int max = splitWidth ? Size.x : Size.y;

            // if leaf is to small, return false
            if (max < (minLeafSize * 2))
                return false;

            int splitAt = Random.Range(minLeafSize, max - minLeafSize);
            if (splitWidth) {
                LeftChild = new Leaf(Pos.x, Pos.y, splitAt, Size.y, depth + 1);
                RightChild = new Leaf(Pos.x + splitAt, Pos.y, Size.x - splitAt, Size.y, depth + 1);
            } else {
                LeftChild = new Leaf(Pos.x, Pos.y, Size.x, splitAt, depth + 1);
                RightChild = new Leaf(Pos.x, Pos.y + splitAt, Size.x, Size.y - splitAt, depth + 1);
            }

            //successful
            return true;
        }

        public Structure BuildRoom() {
            if (LeftChild == null && RightChild == null) {
                Room = new Structure();

                float rand = Random.value;
                if (rand > 0.4f) {
                    Room.Size = new Vector2Int(Random.Range(5, Size.x - 2), Random.Range(5, Size.y - 2));
                } else if (rand > 0.2f) {
                    Room.Size = new Vector2Int(Random.Range(4, Size.x - 2), Random.Range(4, Size.y - 2));
                } else {
                    Room.Size = new Vector2Int(Random.Range(3, Size.x - 2), Random.Range(3, Size.y - 2));
                }
                Room.Pos = new Vector2Int(Pos.x + Random.Range(1, Size.x - Room.Size.x), Pos.y + Random.Range(1, Size.y - Room.Size.y));

                return Room;
            }

            return null;
        }
    }

    public class Map
    {
        public MapTile[][] mapLayout;
        public Vector2Int size;
        public Vector2Int startpos;
        public Vector2Int exitpos;

        public List<Leaf> leaves = new List<Leaf>();
        public List<Structure> rooms = new List<Structure>();
        public List<Structure> tunnels = new List<Structure>();

        public Leaf root;

        public MapTile this[int x, int y] {
            get => mapLayout[x][y];
            set => mapLayout[x][y] = value;
        }

        public Map(int xSize, int ySize) {
            size.x = xSize;
            size.y = ySize;

            mapLayout = new MapTile[xSize][];

            for (int x = 0; x < xSize; x++) {
                mapLayout[x] = new MapTile[ySize];
                for (int y = 0; y < ySize; y++) {
                    mapLayout[x][y] = MapTile.Wall;
                }
            }
        }
    }

    private class Tree
    {
        private readonly Map map = new Map(64, 32);
        //private readonly MapTiles[,] mapTiles = new MapTiles[Settings.MapSize.x, Settings.MapSize.y];

        private Vector2Int startPos;
        private Vector2Int exitPos;

        public Tree() { }

        public Map GenerateNewMap() {
            // create the tree
            CreateTree();

            // create rooms in leaves
            CreateRooms();

            // place player
            PlacePlayer();
            // place exit in random room
            PlaceSpecialTiles();
            // place items
            PlaceItems();

            // connect rooms with tunnels
            CreateTunnels();

            // set map tiles
            SetMaptiles();

            return map;
        }

        private void CreateTree() {
            map.root = new Leaf(0, 0, 64, 32, 0);
            map.leaves.Add(map.root);

            // create all additional leaves
            bool split = true;
            while (split) {
                split = false;

                foreach (Leaf l in map.leaves) {
                    if (!l.SplitTried && l.LeftChild == null && l.RightChild == null) {
                        if (l.Split()) {
                            map.leaves.Add(l.LeftChild);
                            map.leaves.Add(l.RightChild);
                            split = true;
                            break;
                        }
                    }
                }
            }
        }

        private void CreateRooms() {
            foreach (Leaf l in map.leaves) {
                Structure room = l.BuildRoom();
                if (room != null) {
                    map.rooms.Add(room);
                }
            }
        }

        private Vector2Int GetRandomPosInRoom() {
            int randRoom = Random.Range(0, map.rooms.Count);
            return new Vector2Int(map.rooms[randRoom].Pos.x + Random.Range(1, map.rooms[randRoom].Size.x - 1), map.rooms[randRoom].Pos.y + Random.Range(1, map.rooms[randRoom].Size.y - 1));
        }

        private void PlacePlayer() {
            startPos = GetRandomPosInRoom();
        }
        private void PlaceSpecialTiles() {
            do {
                exitPos = GetRandomPosInRoom();
            } while (exitPos == startPos);
        }
        private void PlaceItems() {

        }

        private void CreateTunnels() {
            for (int i = 0; i < map.rooms.Count - 1; i++) {
                Vector2Int tunnelPos = new Vector2Int(Random.Range(map.rooms[i].Pos.x + 1, map.rooms[i].Pos.x + map.rooms[i].Size.x - 1), Random.Range(map.rooms[i].Pos.y + 1, map.rooms[i].Pos.y + map.rooms[i].Size.y - 1));
                Vector2Int secondPoint = new Vector2Int(Random.Range(map.rooms[i + 1].Pos.x + 1, map.rooms[i + 1].Pos.x + map.rooms[i + 1].Size.x - 1), Random.Range(map.rooms[i + 1].Pos.y + 1, map.rooms[i + 1].Pos.y + map.rooms[i + 1].Size.y - 1));

                Vector2Int tunnelSize = new Vector2Int(secondPoint.x - tunnelPos.x, secondPoint.y - tunnelPos.y);

                Structure tunnel = new Structure {
                    Size = tunnelSize,
                    Pos = tunnelPos
                };

                map.tunnels.Add(tunnel);
            }
        }

        private void SetMaptiles() {
            // default the map to wall tiles
            for (int x = 0; x < map.size.x; x++) {
                for (int y = 0; y < map.size.y; y++) {
                    map[x, y] = MapTile.Wall;
                }
            }

            foreach (Leaf leaf in map.leaves) {
                if (leaf.LeftChild == null && leaf.RightChild == null) {
                    for (int x = 0; x < leaf.Room.Size.x; x++) {
                        for (int y = 0; y < leaf.Room.Size.y; y++) {
                            map[leaf.Room.Pos.x + x, leaf.Room.Pos.y + y] = MapTile.Floor;
                        }
                    }
                }
            }

            // set map tiles from tunnels
            foreach (Structure t in map.tunnels) {
                if (t.Size.x > 0) {
                    // split tunnel
                    int rand = t.Size.x / 2;

                    // first part
                    for (int x = 0; x < rand; x++)
                        map[t.Pos.x + x, t.Pos.y] = MapTile.Floor;

                    // 90° angle
                    if (t.Size.y > 0)
                        for (int y = 0; y < t.Size.y; y++)
                            map[t.Pos.x + rand, t.Pos.y + y] = MapTile.Floor;
                    else
                        for (int y = 0; y > t.Size.y; y--)
                            map[t.Pos.x + rand, t.Pos.y + y] = MapTile.Floor;

                    // second part
                    for (int x = rand; x < t.Size.x; x++)
                        map[t.Pos.x + x, t.Pos.y + t.Size.y] = MapTile.Floor;
                } else {
                    // split tunnel
                    int rand = t.Size.x / 2;

                    // first part
                    for (int x = 0; x > rand; x--)
                        map[t.Pos.x + x, t.Pos.y] = MapTile.Floor;

                    // 90° angle
                    if (t.Size.y > 0)
                        for (int y = 0; y < t.Size.y; y++)
                            map[t.Pos.x + rand, t.Pos.y + y] = MapTile.Floor;
                    else
                        for (int y = 0; y > t.Size.y; y--)
                            map[t.Pos.x + rand, t.Pos.y + y] = MapTile.Floor;

                    // second part
                    for (int x = rand; x > t.Size.x; x--)
                        map[t.Pos.x + x, t.Pos.y + t.Size.y] = MapTile.Floor;
                }
            }
        }
    }
}
