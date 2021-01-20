// using Mirror;
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
    private GameObject prefabDoorLR;
    [SerializeField]
    private GameObject prefabDoorUD;

    [Header("Settings")]
    [SerializeField]
    private Vector2Int maxSize = Vector2Int.one;

    private GameObject[] doors = new GameObject[0];
    private GameObject[] objects = new GameObject[0];

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

    /*
    [SyncVar]
    private int seed;

    private void Awake() {
        if (isServer)
            seed = Random.Range(int.MinValue, int.MaxValue);
    }

    public override void OnStartClient() {
        base.OnStartClient();

        CreateDungeon(seed);
    }
     */

    private void OnValidate() {
        // clamp maxSize
        if (maxSize.x < 16) maxSize.x = 16;
        if (maxSize.y < 16) maxSize.y = 16;
        if (maxSize.x > 1024) maxSize.x = 1024;
        if (maxSize.y > 1024) maxSize.y = 1024;
    }

    public void CreateDungeon(int seed) {
        if (this.doors.Length > 0) {
            for (int i = this.doors.Length - 1; i >= 0; i--) {
                Destroy(this.doors[i]);
            }
        }
        if (objects.Length > 0) {
            for (int i = objects.Length - 1; i >= 0; i--) {
                Destroy(objects[i]);
            }
        }

        List<Fast2DArray<int>> roomLayouts = new List<Fast2DArray<int>>();
        List<List<TiledImporter.PrefabContainer>> roomGameObjects = new List<List<TiledImporter.PrefabContainer>>();

        int mapCount = 6;
        List<TiledImporter.PrefabContainer> gos;
        for (int i = 1; i < mapCount; i++) {
            roomLayouts.Add(TiledImporter.Instance.GetReplacableMap("room" + i.ToString(), out PropertyDict properties, out gos));
            
            // Example:
            if (properties.TryGetValue("roomType", out string value) == false)
                throw new System.Exception("No room type in map: room" + i + "!");
            
            if (int.TryParse(value, out int roomType) == false)
                throw new System.Exception("Room type is not an integer in: room" + i + "!");
            
            // do something with the roomType here

            roomGameObjects.Add(gos);
        }

        dungeon = new Dungeon(maxSize.x, maxSize.y, roomLayouts.ToArray(), roomGameObjects.ToArray(), 10, seed);

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

        // place doors
        Dungeon.Door[] doors = dungeon.GetDoorLocations();
        this.doors = new GameObject[doors.Length];
        for (int i = 0; i < doors.Length; i++) {
            if (doors[i].LeftRight) {
                GameObject d = Instantiate(prefabDoorLR, new Vector3(doors[i].Position.x + 0.5f, doors[i].Position.y + 0.5f, 0f), Quaternion.identity);
                d.transform.parent = objectContainer;
                this.doors[i] = d;
            } else {
                GameObject d = Instantiate(prefabDoorUD, new Vector3(doors[i].Position.x + 0.5f, doors[i].Position.y + 0.5f, 0f), Quaternion.identity);
                d.transform.parent = objectContainer;
                this.doors[i] = d;
            }
        }

        // place GameObjects
        List<GameObject> objs = new List<GameObject>();
        foreach (var room in dungeon.Rooms) {
            foreach (var prefabContainer in room.gameObjects) {
                GameObject go = Instantiate(prefabContainer.Prefab);
                go.transform.position = new Vector3(room.Position.x, room.Position.y, 0f) + prefabContainer.Position;
                go.transform.parent = objectContainer;
                objs.Add(go);
            }
        }
        objects = objs.ToArray();

        if (Player.LocalPlayer) // check to allow for debugging if a localplayer is not scene
            Player.LocalPlayer.StateCommunicator.CmdLevelSetLoaded(true);
    }
}
