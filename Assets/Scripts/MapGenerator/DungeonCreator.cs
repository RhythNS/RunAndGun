using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using MapGenerator;

public class DungeonCreator : NetworkBehaviour
{
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

    [Header("Settings")]
    [SerializeField]
    private Vector2Int maxSize = Vector2Int.one;

    [SerializeField]
    [Range(3, 24)]
    private int minRoomSize = 3;
    [SerializeField]
    [Range(3, 24)]
    private int maxRoomSize = 9;

    [SerializeField]
    [Range(3, 24)]
    private int minCorridorLength = 3;
    [SerializeField]
    [Range(3, 24)]
    private int maxCorridorLength = 7;

    [SerializeField]
    [Range(1, 100)]
    private int maxStructures = 50;
    [SerializeField]
    [Range(0f, 1f)]
    private float roomChance = 0.4f;

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

    private void OnValidate() {
        // clamp maxSize
        if (maxSize.x < 16) maxSize.x = 16;
        if (maxSize.y < 16) maxSize.y = 16;
        if (maxSize.x > 1024) maxSize.x = 1024;
        if (maxSize.y > 1024) maxSize.y = 1024;

        // make sure min is below max
        if (minRoomSize > maxRoomSize)
            minRoomSize = maxRoomSize;
        if (minCorridorLength > maxCorridorLength)
            minCorridorLength = maxCorridorLength;
    }

    public void CreateDungeon(int seed) {
        List<Fast2DArray<int>> roomLayouts = new List<Fast2DArray<int>>();
        List<List<GameObject>> roomGameObjects = new List<List<GameObject>>();

        int mapCount = 4;
        List<GameObject> gos;
        for (int i = 1; i < mapCount; i++) {
            roomLayouts.Add(TiledImporter.Instance.GetReplacableMap("room" + i.ToString(), 0, 0, out gos));
            roomGameObjects.Add(gos);
        }

        dungeon = new Dungeon(maxSize.x, maxSize.y, roomLayouts.ToArray(), roomGameObjects.ToArray(), 50, seed);

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

        // place GameObjects
        foreach (var room in dungeon.Rooms) {
            foreach (var go in room.gameObjects) {

            }
        }
    }
}
