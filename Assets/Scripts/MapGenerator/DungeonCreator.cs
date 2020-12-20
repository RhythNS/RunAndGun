using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    private RuleTile tileFloor;
    [SerializeField]
    private RuleTile tileWall;
    [SerializeField]
    private RuleTile tileCeiling;
    [SerializeField]
    private Tile tilePlaceholder;

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
        if (dungeon != null)
            dungeon.Reset();

        dungeon = new Dungeon(maxSize.x, maxSize.y, seed, minRoomSize, maxRoomSize, minCorridorLength, maxCorridorLength, maxStructures, roomChance);

        // clear tilemaps
        tilemapFloor.ClearAllTiles();
        tilemapWall.ClearAllTiles();
        tilemapCeiling.ClearAllTiles();

        // create new tilemaps
        Vector3Int[] positionsFloor = new Vector3Int[dungeon.Size.x * dungeon.Size.y];
        Vector3Int[] positionsWall = new Vector3Int[dungeon.Size.x * dungeon.Size.y];
        Vector3Int[] positionsCeiling = new Vector3Int[dungeon.Size.x * dungeon.Size.y];
        TileBase[] tilesFloor = new TileBase[dungeon.Size.x * dungeon.Size.y];
        TileBase[] tilesWall = new TileBase[dungeon.Size.x * dungeon.Size.y];
        TileBase[] tilesCeiling = new TileBase[dungeon.Size.x * dungeon.Size.y];

        int index;
        for (int x = 0; x < dungeon.Size.x; x++) {
            for (int y = 0; y < dungeon.Size.y; y++) {
                index = x * dungeon.Size.y + y;

                if (dungeon[x, y] == Map.TileType.Floor) {
                    positionsFloor[index] = new Vector3Int(x, y, 0);
                    tilesFloor[index] = tileFloor;
                } else {
                    if (y >= 2 && (dungeon[x, y - 1] == Map.TileType.Floor || dungeon[x, y - 2] == Map.TileType.Floor)) {
                        positionsWall[index] = new Vector3Int(x, y, 0);
                        tilesWall[index] = tileWall;
                    } else {
                        positionsCeiling[index] = new Vector3Int(x, y, 0);
                        tilesCeiling[index] = tileCeiling;
                    }
                }
            }
        }

        // set tiles
        tilemapFloor.SetTiles(positionsFloor, tilesFloor);
        tilemapWall.SetTiles(positionsWall, tilesWall);
        tilemapCeiling.SetTiles(positionsCeiling, tilesCeiling);
    }
}
