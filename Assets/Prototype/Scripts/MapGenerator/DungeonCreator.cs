using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonCreator : MonoBehaviour
{
    [HideInInspector]
    public Dungeon dungeon;

    [SerializeField]
    private Tilemap tilemap;

    [SerializeField]
    private Tile wall;
    [SerializeField]
    private Tile floor;

    [Header("Dungeon")]
    [SerializeField]
    private int seed = -1;

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

    public void CreateDungeon() {
        if (dungeon != null)
            dungeon.Reset();

        dungeon = new Dungeon(maxSize.x, maxSize.y, seed, minRoomSize, maxRoomSize, minCorridorLength, maxCorridorLength, maxStructures, roomChance);

        for (int x = 0; x < dungeon.SizeX; x++) {
            for (int y = 0; y < dungeon.SizeY; y++) {
                if (dungeon[x, y] == Map.Tile.Wall) {
                    tilemap.SetTile(new Vector3Int(x, y, 0), wall);
                } else if (dungeon[x, y] == Map.Tile.Floor) {
                    tilemap.SetTile(new Vector3Int(x, y, 0), floor);
                }
            }
        }
    }
}
