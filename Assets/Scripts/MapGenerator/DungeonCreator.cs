using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using MapGenerator;

public class DungeonCreator : NetworkBehaviour
{
    [HideInInspector]
    public Dungeon_OLD dungeon;
    
    [Header("Tilemaps")]
    [SerializeField]
    private Tilemap tilemapFloor;
    [SerializeField]
    private Tilemap tilemapWall;
    [SerializeField]
    private Tilemap tilemapCeiling;

    [SerializeField]
    private Tileset tileset;

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

        dungeon = new Dungeon_OLD(maxSize.x, maxSize.y, seed, minRoomSize, maxRoomSize, minCorridorLength, maxCorridorLength, maxStructures, roomChance);

        // clear tilemaps
        tilemapFloor.ClearAllTiles();
        tilemapWall.ClearAllTiles();
        tilemapCeiling.ClearAllTiles();

        //// create new tilemaps
        //int tileCount = dungeon.Size.x * dungeon.Size.y;
        //Vector3Int[] positionsFloor = new Vector3Int[tileCount];
        //Vector3Int[] positionsWall = new Vector3Int[tileCount];
        //Vector3Int[] positionsCeiling = new Vector3Int[tileCount];
        //TileBase[] tilesFloor = new TileBase[tileCount];
        //TileBase[] tilesWall = new TileBase[tileCount];
        //TileBase[] tilesCeiling = new TileBase[tileCount];

        //int index;
        //for (int x = 0; x < dungeon.Size.x; x++) {
        //    for (int y = 0; y < dungeon.Size.y; y++) {
        //        index = x * dungeon.Size.y + y;

        //        if (dungeon[x, y] == Map.TileType.Floor) {
        //            positionsFloor[index] = new Vector3Int(x, y, 0);
        //            tilesFloor[index] = tileset.tileFloor;

        //            if (y >= 2 && dungeon[x, y - 2] == Map.TileType.Wall) {
        //                positionsCeiling[index] = new Vector3Int(x, y, 0);
        //                tilesCeiling[index] = tileset.tileCeiling;
        //            }
        //        } else {
        //            if (y >= 2 && (dungeon[x, y - 1] == Map.TileType.Floor)) {
        //                positionsWall[index] = new Vector3Int(x, y, 0);
        //                tilesWall[index] = tileset.tileWall;

        //                if (dungeon[x, y - 2] == Map.TileType.Wall) {
        //                    positionsCeiling[index] = new Vector3Int(x, y, 0);
        //                    tilesCeiling[index] = tileset.tileCeiling;
        //                }
        //            } else if (y >= 2 && dungeon[x, y - 1] != Map.TileType.Floor && dungeon[x, y - 2] != Map.TileType.Floor) {
        //                positionsCeiling[index] = new Vector3Int(x, y, 0);
        //                tilesCeiling[index] = tileset.tileCeiling;
        //            }
        //        }
        //    }
        //}

        // create new tilemaps
        int tileCount = dungeon.Size.x * dungeon.Size.y * 4;
        Vector3Int[] positionsFloor = new Vector3Int[tileCount];
        Vector3Int[] positionsWall = new Vector3Int[tileCount];
        Vector3Int[] positionsCeiling = new Vector3Int[tileCount];
        TileBase[] tilesFloor = new TileBase[tileCount];
        TileBase[] tilesWall = new TileBase[tileCount];
        TileBase[] tilesCeiling = new TileBase[tileCount];

        int index, xPos, yPos;
        for (int x = 0; x < dungeon.Size.x; x++) {
            for (int y = 0; y < dungeon.Size.y; y++) {
                index = (x * dungeon.Size.y + y) * 4;
                xPos = x * 2;
                yPos = y * 2;

                if (dungeon[x, y] == Map_OLD.TileType.Floor) {
                    positionsFloor[index] = new Vector3Int(xPos, yPos, 0);
                    positionsFloor[index + 1] = new Vector3Int(xPos + 1, yPos, 0);
                    positionsFloor[index + 2] = new Vector3Int(xPos, yPos + 1, 0);
                    positionsFloor[index + 3] = new Vector3Int(xPos + 1, yPos + 1, 0);
                    tilesFloor[index] = tileset.tileFloor;
                    tilesFloor[index + 1] = tileset.tileFloor;
                    tilesFloor[index + 2] = tileset.tileFloor;
                    tilesFloor[index + 3] = tileset.tileFloor;

                    if (y >= 2 && dungeon[x, y - 1] == Map_OLD.TileType.Wall) {
                        positionsCeiling[index] = new Vector3Int(xPos, yPos, 0);
                        positionsCeiling[index + 1] = new Vector3Int(xPos + 1, yPos, 0);
                        positionsCeiling[index + 2] = new Vector3Int(xPos, yPos + 1, 0);
                        positionsCeiling[index + 3] = new Vector3Int(xPos + 1, yPos + 1, 0);
                        tilesCeiling[index] = tileset.tileCeiling;
                        tilesCeiling[index + 1] = tileset.tileCeiling;
                        tilesCeiling[index + 2] = tileset.tileCeiling;
                        tilesCeiling[index + 3] = tileset.tileCeiling;
                    }
                } else {
                    if (y >= 2 && (dungeon[x, y - 1] == Map_OLD.TileType.Floor)) {
                        positionsWall[index] = new Vector3Int(xPos, yPos, 0);
                        positionsWall[index + 1] = new Vector3Int(xPos + 1, yPos, 0);
                        positionsWall[index + 2] = new Vector3Int(xPos, yPos + 1, 0);
                        positionsWall[index + 3] = new Vector3Int(xPos + 1, yPos + 1, 0);
                        tilesWall[index] = tileset.tileWall;
                        tilesWall[index + 1] = tileset.tileWall;
                        tilesWall[index + 2] = tileset.tileWall;
                        tilesWall[index + 3] = tileset.tileWall;

                        if (dungeon[x, y - 1] == Map_OLD.TileType.Wall) {
                            positionsCeiling[index] = new Vector3Int(xPos, yPos, 0);
                            positionsCeiling[index + 1] = new Vector3Int(xPos + 1, yPos, 0);
                            positionsCeiling[index + 2] = new Vector3Int(xPos, yPos + 1, 0);
                            positionsCeiling[index + 3] = new Vector3Int(xPos + 1, yPos + 1, 0);
                            tilesCeiling[index] = tileset.tileCeiling;
                            tilesCeiling[index + 1] = tileset.tileCeiling;
                            tilesCeiling[index + 2] = tileset.tileCeiling;
                            tilesCeiling[index + 3] = tileset.tileCeiling;
                        }
                    } else if (y >= 2 && dungeon[x, y - 1] != Map_OLD.TileType.Floor) {
                        positionsCeiling[index] = new Vector3Int(xPos, yPos, 0);
                        positionsCeiling[index + 1] = new Vector3Int(xPos + 1, yPos, 0);
                        positionsCeiling[index + 2] = new Vector3Int(xPos, yPos + 1, 0);
                        positionsCeiling[index + 3] = new Vector3Int(xPos + 1, yPos + 1, 0);
                        tilesCeiling[index] = tileset.tileCeiling;
                        tilesCeiling[index + 1] = tileset.tileCeiling;
                        tilesCeiling[index + 2] = tileset.tileCeiling;
                        tilesCeiling[index + 3] = tileset.tileCeiling;
                    }
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
        for (int x = -10; x < dungeon.Size.x * 2 + 10; x++) {
            for (int y = -10; y < dungeon.Size.y * 2 + 10; y++) {
                if (x < 0 || y < 4 || x >= dungeon.Size.x * 2 || y >= dungeon.Size.y * 2) {
                    positions.Add(new Vector3Int(x, y, 0));
                    tiles.Add(tileset.tileCeiling);
                }
            }
        }

        tilemapCeiling.SetTiles(positions.ToArray(), tiles.ToArray());
    }
}
