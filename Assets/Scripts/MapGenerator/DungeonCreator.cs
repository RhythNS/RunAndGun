using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using MapGenerator;
using TiledSharp;

public class DungeonCreator : MonoBehaviour
{
    public static DungeonCreator Instance { get; private set; }

    [HideInInspector]
    public Dungeon dungeon;

    [Header("Tilemaps")]
    [SerializeField]
    private Tilemap tilemapFloor;
    [SerializeField]
    private Tilemap tilemapWall;
    [SerializeField]
    private Tilemap tilemapCeiling;

    [SerializeField]
    private Tileset tileset;
    [SerializeField]
    private Tile tilePlaceHolder;

    [SerializeField]
    private Transform objectContainer;
    [SerializeField]
    private Transform roomsContainer;

    [SerializeField]
    private GameObject prefabDungeonRoom;
    [SerializeField]
    private GameObject prefabDoorLR;
    [SerializeField]
    private GameObject prefabDoorUD;

    [SerializeField]
    private Transform mask;

    [SerializeField]
    private EnemyObject[] enemyObjects;
    [SerializeField]
    private Pickable[] pickableObjects;

    [Header("Settings")]
    [SerializeField]
    private Vector2Int maxSize = Vector2Int.one;

    public List<DungeonRoom> dungeonRooms = new List<DungeonRoom>();

    private float loadStatus = 0.0f;
    public float LoadStatus {
        get {
            return loadStatus;
        }
    }

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("Already a DungeonCreator in scene! Deleting myself!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public IEnumerator CreateLevel(int levelNumber)
    {
        return CreateDungeon(GameManager.gameMode.levelSeeds[levelNumber]);
    }

    public IEnumerator CreateDungeon(int seed)
    {
        if (roomsContainer.childCount > 0) {
            for (int i = roomsContainer.childCount - 1; i >= 0; i--) {
                Destroy(roomsContainer.GetChild(i).gameObject);
            }
        }

        yield return new WaitForEndOfFrame();

        dungeonRooms = new List<DungeonRoom>();

        List<Fast2DArray<int>> roomLayouts = new List<Fast2DArray<int>>();
        List<List<TiledImporter.PrefabLocations>> roomGameObjects = new List<List<TiledImporter.PrefabLocations>>();
        List<RoomType> roomTypes = new List<RoomType>();

        roomLayouts.Add(TiledImporter.Instance.GetReplacableMap("startRoom", out PropertyDict properties, out List<TiledImporter.PrefabLocations> gos));
        if (properties.TryGetValue("roomType", out string value) == false)
            throw new System.Exception("No room type in map: startRoom!");
        if (int.TryParse(value, out int roomType) == false)
            throw new System.Exception("Room type is not an integer in: startRoom!");
        roomTypes.Add((RoomType)roomType);
        roomGameObjects.Add(gos);

        roomLayouts.Add(TiledImporter.Instance.GetReplacableMap("bossRoom", out properties, out gos));
        if (properties.TryGetValue("roomType", out value) == false)
            throw new System.Exception("No room type in map: bossRoom!");
        if (int.TryParse(value, out roomType) == false)
            throw new System.Exception("Room type is not an integer in: bossRoom!");
        roomTypes.Add((RoomType)roomType);
        roomGameObjects.Add(gos);

        int mapCount = 7;
        for (int i = 1; i <= mapCount; i++) {
            roomLayouts.Add(TiledImporter.Instance.GetReplacableMap("room" + i.ToString(), out properties, out gos));

            // Example:
            if (properties.TryGetValue("roomType", out value) == false)
                throw new System.Exception("No room type in map: room" + i + "!");

            if (int.TryParse(value, out roomType) == false)
                throw new System.Exception("Room type is not an integer in: room" + i + "!");

            // do something with the roomType here
            roomTypes.Add((RoomType)roomType);

            roomGameObjects.Add(gos);
        }

        yield return new WaitForEndOfFrame();

        dungeon = new Dungeon(maxSize.x, maxSize.y, roomLayouts.ToArray(), roomGameObjects.ToArray(), roomTypes.ToArray(), 10, seed);

        // adjust mask size
        mask.localScale = new Vector3(dungeon.Size.x, dungeon.Size.y, 1f);
        mask.position = this.transform.position + (mask.localScale / 2f);

        yield return new WaitForEndOfFrame();

        // clear tilemaps
        tilemapFloor.ClearAllTiles();

        yield return new WaitForEndOfFrame();

        tilemapWall.ClearAllTiles();
        tilemapCeiling.ClearAllTiles();

        yield return new WaitForEndOfFrame();

        // create new tilemaps
        Vector3Int[] positionsFloor;
        Vector3Int[] positionsWall;
        Vector3Int[] positionsCeiling;
        TileBase[] tilesFloor;
        TileBase[] tilesWall;
        TileBase[] tilesCeiling;

        int indexFloor;
        int indexWall;
        int indexCeil;

        for (int x = 0; x < dungeon.Size.x; x++) {
            positionsFloor = new Vector3Int[dungeon.Size.y];
            positionsWall = new Vector3Int[dungeon.Size.y];
            positionsCeiling = new Vector3Int[dungeon.Size.y];
            tilesFloor = new TileBase[dungeon.Size.y];
            tilesWall = new TileBase[dungeon.Size.y];
            tilesCeiling = new TileBase[dungeon.Size.y];
            indexFloor = 0;
            indexWall = 0;
            indexCeil = 0;

            for (int y = 0; y < dungeon.Size.y; y++) {
                positionsFloor[indexFloor] = new Vector3Int(x, y, 0);
                if (dungeon[x, y] == TileType.Floor)
                    tilesFloor[indexFloor] = tileset.tileFloor;
                else
                    tilesFloor[indexFloor] = tilePlaceHolder;
                indexFloor++;

                if (y >= 1 && dungeon[x, y] == TileType.Wall && dungeon[x, y - 1] == TileType.Floor) {
                    positionsWall[indexWall] = new Vector3Int(x, y, 0);
                    tilesWall[indexWall] = tileset.tileWall;

                    indexWall++;
                }

                if (y >= 2 && dungeon[x, y - 2] == TileType.Wall) {
                    positionsCeiling[indexCeil] = new Vector3Int(x, y, 0);
                    tilesCeiling[indexCeil] = tileset.tileCeiling;

                    indexCeil++;
                }
            }

            // set tiles
            tilemapFloor.SetTiles(positionsFloor, tilesFloor);

            yield return new WaitForEndOfFrame();

            tilemapWall.SetTiles(positionsWall, tilesWall);

            tilemapCeiling.SetTiles(positionsCeiling, tilesCeiling);

            yield return new WaitForEndOfFrame();

            loadStatus += 1.0f / dungeon.Size.x;
        }

        // set border tiles
        List<Vector3Int> positions = new List<Vector3Int>();
        List<TileBase> tiles = new List<TileBase>();
        for (int x = -10; x < dungeon.Size.x + 10; x++) {
            for (int y = -10; y < 2; y++) {
                positions.Add(new Vector3Int(x, y, 0));
                tiles.Add(tileset.tileCeiling);
            }
        }
        tilemapCeiling.SetTiles(positions.ToArray(), tiles.ToArray());
        positions.Clear();
        tiles.Clear();
        yield return new WaitForEndOfFrame();

        for (int x = -10; x < dungeon.Size.x + 10; x++) {
            for (int y = dungeon.Size.y; y < dungeon.Size.y + 10; y++) {
                positions.Add(new Vector3Int(x, y, 0));
                tiles.Add(tileset.tileCeiling);
            }
        }
        tilemapCeiling.SetTiles(positions.ToArray(), tiles.ToArray());
        positions.Clear();
        tiles.Clear();
        yield return new WaitForEndOfFrame();

        for (int x = -10; x < 0; x++) {
            for (int y = 0; y < dungeon.Size.y; y++) {
                positions.Add(new Vector3Int(x, y, 0));
                tiles.Add(tileset.tileCeiling);
            }
        }
        tilemapCeiling.SetTiles(positions.ToArray(), tiles.ToArray());
        positions.Clear();
        tiles.Clear();
        yield return new WaitForEndOfFrame();

        for (int x = dungeon.Size.x; x < dungeon.Size.x + 10; x++) {
            for (int y = 0; y < dungeon.Size.y; y++) {
                positions.Add(new Vector3Int(x, y, 0));
                tiles.Add(tileset.tileCeiling);
            }
        }
        tilemapCeiling.SetTiles(positions.ToArray(), tiles.ToArray());
        positions.Clear();
        tiles.Clear();
        yield return new WaitForEndOfFrame();


        // set rooms
        dungeonRooms = new List<DungeonRoom>();
        DungeonDict.Instance.ResetRooms(dungeon.Rooms.Length);

        for (int i = 0; i < dungeon.Rooms.Length; i++) {
            GameObject go = Instantiate(prefabDungeonRoom);
            go.transform.parent = roomsContainer;

            // set room type
            DungeonRoom dr = null;
            switch (dungeon.Rooms[i].Type) {
                case RoomType.Start:
                    StartRoom sr = go.AddComponent<StartRoom>();
                    if (Player.LocalPlayer.isServer) {
                        sr.SpawnItems(new Vector3(128, 128, 0));
                    }

                    dr = sr;

                    break;

                case RoomType.Combat:
                    CombatRoom cr = go.AddComponent<CombatRoom>();
                    cr.ThreatLevel = dungeon.Rooms[i].TileCount;
                    cr.enemiesToSpawn = new EnemyObject[cr.ThreatLevel / 48];
                    for (int j = 0; j < cr.enemiesToSpawn.Length; j++) {
                        cr.enemiesToSpawn[j] = enemyObjects[Random.Range(0, enemyObjects.Length)];
                    }
                    dr = cr;
                    break;
                    
                case RoomType.Loot:
                    LootRoom lr = go.AddComponent<LootRoom>();
                    lr.pickables = new Pickable[dungeon.Rooms[i].TileCount / 48];
                    for (int j = 0; j < lr.pickables.Length; j++) {
                        lr.pickables[j] = pickableObjects[Random.Range(0, pickableObjects.Length)];
                    }
                    dr = lr;
                    break;

                case RoomType.Shop:
                    dr = go.AddComponent<ShopRoom>();
                    break;

                case RoomType.Boss:
                    dr = go.AddComponent<BossRoom>();
                    break;

                default:
                    break;
            }

            if (dr != null) {
                // set room id
                dr.id = i;
                DungeonDict.Instance.Register(dr);

                // set room border
                dr.Border = new Rect(dungeon.Rooms[i].Position.x, dungeon.Rooms[i].Position.y, dungeon.Rooms[i].Layout.XSize, dungeon.Rooms[i].Layout.YSize);
                dr.walkableTiles = dungeon.GetWalkableTiles(i);

                // set room objects
                List<GameObject> objs = new List<GameObject>();
                foreach (var prefabContainer in dungeon.Rooms[i].GameObjects) {
                    GameObject obj = Instantiate(prefabContainer.Prefab);
                    obj.transform.position = new Vector3(dungeon.Rooms[i].Position.x, dungeon.Rooms[i].Position.y, 0f) + prefabContainer.Position;
                    obj.transform.parent = go.transform;
                    objs.Add(obj);
                }
                dr.objects = objs;

                // set room doors
                DoorLocations[] doorLocs = dungeon.GetDoorsOfRoom(i);
                foreach (var doorLoc in doorLocs) {
                    if (doorLoc.IsLeftRight) {
                        GameObject d = Instantiate(prefabDoorLR, new Vector3(doorLoc.Position.x + 0.5f, doorLoc.Position.y + 0.5f, 0f), Quaternion.identity);
                        d.transform.parent = go.transform;

                        DungeonDoor dd = d.AddComponent<DungeonDoor>();
                        dd.IsLeftRight = true;
                        if (i == 0)
                            dd.IsLocked = false;
                        dr.doors.Add(dd);
                    } else {
                        GameObject d = Instantiate(prefabDoorUD, new Vector3(doorLoc.Position.x + 0.5f, doorLoc.Position.y + 0.5f, 0f), Quaternion.identity);
                        d.transform.parent = go.transform;

                        DungeonDoor dd = d.AddComponent<DungeonDoor>();
                        dd.IsLeftRight = false;
                        if (i == 0)
                            dd.IsLocked = false;
                        dr.doors.Add(dd);
                    }
                }

                dungeonRooms.Add(dr);
            } else {
                throw new System.Exception("No DungeonRoom designated...");
            }

            yield return new WaitForEndOfFrame();
        }

        if (Player.LocalPlayer) // check to allow for debugging if a localplayer is not scene
            Player.LocalPlayer.StateCommunicator.CmdLevelSetLoaded(true);

        loadStatus = 1.0f;
    }

    public void AdjustMask(Vector3 position, Vector3 scale) {
        mask.localScale = scale + new Vector3(0f, 1.5f, 0f);
        mask.position = position + scale / 2f + new Vector3(0f, 1f, 0f);
    }

    public void ResetMask() {
        mask.localScale = new Vector3(dungeon.Size.x, dungeon.Size.y, 1f);
        mask.position = this.transform.position + (mask.localScale / 2f);
    }

    public Vector3 TilePositionToWorldPositionMiddle(Vector2Int pos)
    {
        return tilemapFloor.CellToWorld((Vector3Int)pos) + tilemapFloor.cellSize * 0.5f;
    }

    public Vector3 TilePositionToWorldPositionMiddle(int x, int y)
    {
        return tilemapFloor.CellToWorld(new Vector3Int(x,y,0)) + tilemapFloor.cellSize * 0.5f;
    }
    public Vector3 TilePositionToWorldPosition(Vector2Int pos) {
        // return this.transform.position + new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0f);
        return tilemapFloor.CellToWorld((Vector3Int)pos);
    }

    public Vector2Int WorldPositionToTilePosition(Vector3 pos) {
        //        return new Vector2Int((int)(pos.x - this.transform.position.x), (int)(pos.y - this.transform.position.y));
        return (Vector2Int)tilemapFloor.WorldToCell(pos);
    }
}
