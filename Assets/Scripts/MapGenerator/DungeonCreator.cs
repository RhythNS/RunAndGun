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

    [Header("Settings")]
    [SerializeField]
    private Vector2Int maxSize = Vector2Int.one;

    public List<DungeonRoom> dungeonRooms = new List<DungeonRoom>();

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

    public void CreateDungeon(int seed) {
        if (roomsContainer.childCount > 0) {
            for (int i = roomsContainer.childCount - 1; i >= 0; i--) {
                Destroy(roomsContainer.GetChild(i).gameObject);
            }
        }

        dungeonRooms = new List<DungeonRoom>();

        List<Fast2DArray<int>> roomLayouts = new List<Fast2DArray<int>>();
        List<List<TiledImporter.PrefabLocations>> roomGameObjects = new List<List<TiledImporter.PrefabLocations>>();
        List<RoomType> roomTypes = new List<RoomType>();

        int mapCount = 6;
        for (int i = 1; i <= mapCount; i++) {
            roomLayouts.Add(TiledImporter.Instance.GetReplacableMap("room" + i.ToString(), out PropertyDict properties, out List<TiledImporter.PrefabLocations> gos));

            // Example:
            if (properties.TryGetValue("roomType", out string value) == false)
                throw new System.Exception("No room type in map: room" + i + "!");

            if (int.TryParse(value, out int roomType) == false)
                throw new System.Exception("Room type is not an integer in: room" + i + "!");

            // do something with the roomType here
            roomTypes.Add((RoomType)roomType);

            roomGameObjects.Add(gos);
        }

        dungeon = new Dungeon(maxSize.x, maxSize.y, roomLayouts.ToArray(), roomGameObjects.ToArray(), roomTypes.ToArray(), 10, seed);

        // adjust mask size
        mask.localScale = new Vector3(dungeon.Size.x, dungeon.Size.y, 1f);
        mask.position = this.transform.position + (mask.localScale / 2f);

        // clear tilemaps
        tilemapFloor.ClearAllTiles();
        tilemapWall.ClearAllTiles();
        tilemapCeiling.ClearAllTiles();

        // create new tilemaps
        int tileCount = dungeon.Size.x * dungeon.Size.y;
        Vector3Int[] positionsFloor = new Vector3Int[tileCount];
        Vector3Int[] positionsWall = new Vector3Int[tileCount];
        Vector3Int[] positionsCeiling = new Vector3Int[tileCount];
        TileBase[] tilesFloor = new TileBase[tileCount];
        TileBase[] tilesWall = new TileBase[tileCount];
        TileBase[] tilesCeiling = new TileBase[tileCount];

        int indexFloor = 0;
        int indexWall = 0;
        int indexCeil = 0;
        for (int x = 0; x < dungeon.Size.x; x++) {
            for (int y = 0; y < dungeon.Size.y; y++) {
                if (dungeon[x, y] == TileType.Floor) {
                    positionsFloor[indexFloor] = new Vector3Int(x, y, 0);
                    tilesFloor[indexFloor] = tileset.tileFloor;

                    indexFloor++;
                } else {
                    positionsFloor[indexFloor] = new Vector3Int(x, y, 0);
                    tilesFloor[indexFloor] = tilePlaceHolder;

                    indexFloor++;
                }

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
        }

        // set tiles
        tilemapFloor.SetTiles(positionsFloor, tilesFloor);
        tilemapWall.SetTiles(positionsWall, tilesWall);
        tilemapCeiling.SetTiles(positionsCeiling, tilesCeiling);

        // set border tiles
        List<Vector3Int> positions = new List<Vector3Int>();
        List<TileBase> tiles = new List<TileBase>();
        for (int x = -10; x < dungeon.Size.x + 10; x++) {
            for (int y = -10; y < dungeon.Size.y + 10; y++) {
                if (x < 0 || y < 0 || x >= dungeon.Size.x || y >= dungeon.Size.y) {
                    positions.Add(new Vector3Int(x, y, 0));
                    tiles.Add(tileset.tileCeiling);
                }
            }
        }
        tilemapCeiling.SetTiles(positions.ToArray(), tiles.ToArray());

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
                    dr = go.AddComponent<StartRoom>();
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
                    dr = go.AddComponent<LootRoom>();
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
        }

        if (Player.LocalPlayer) // check to allow for debugging if a localplayer is not scene
            Player.LocalPlayer.StateCommunicator.CmdLevelSetLoaded(true);
    }

    public void AdjustMask(Vector3 position, Vector3 scale) {
        mask.localScale = scale;
        mask.position = position + scale / 2f;
    }

    public void ResetMask() {
        mask.localScale = new Vector3(dungeon.Size.x, dungeon.Size.y, 1f);
        mask.position = this.transform.position + (mask.localScale / 2f);
    }

    public Vector3 TilePositionToWorldPosition(Vector2Int pos) {
        return this.transform.position + new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0f);
    }

    public Vector2Int WorldPositionToTilePosition(Vector3 pos) {
        return new Vector2Int((int)(pos.x - this.transform.position.x), (int)(pos.y - this.transform.position.y));
    }
}
