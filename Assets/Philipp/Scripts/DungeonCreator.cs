using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonCreator : MonoBehaviour
{
    [HideInInspector]
    public Dungeon dungeon;

    [SerializeField]
    private Vector2Int maxSize = Vector2Int.one;

    [SerializeField]
    private Tilemap tilemap;

    [SerializeField]
    private Tile wall;
    [SerializeField]
    private Tile floor;

    [SerializeField]
    public int seed = -1;

    public void CreateDungeon() {
        if (dungeon != null)
            dungeon.Reset();

        dungeon = new Dungeon(maxSize.x, maxSize.y, seed);

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
