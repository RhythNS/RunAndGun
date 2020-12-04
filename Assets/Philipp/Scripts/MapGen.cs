using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGen : MonoBehaviour
{
    [SerializeField]
    private Tilemap tilemap;

    [SerializeField]
    private Tile wall;
    [SerializeField]
    private Tile floor;

    public int seed = -1;

    MapGenerator.Map map;

    public void GenerateMap() {
        map = MapGenerator.GenerateMap(seed);

        for (int x = 0; x < map.size.x; x++) {
            for (int y = 0; y < map.size.y; y++) {
                if (map[x, y] == MapGenerator.MapTile.Wall) {
                    tilemap.SetTile(new Vector3Int(x, y, 0), wall);
                } else if (map[x, y] == MapGenerator.MapTile.Floor) {
                    tilemap.SetTile(new Vector3Int(x, y, 0), floor);
                }
            }
        }
    }
}
