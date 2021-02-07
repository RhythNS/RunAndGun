using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CreateColliderFromRoom : MonoBehaviour
{
    [SerializeField] private Tileset tileset;

    private void Start()
    {
        DungeonRoom room = GetComponent<DungeonRoom>();

        Rect border = room.Border;


        BoxCollider2D col;
        col = gameObject.AddComponent<BoxCollider2D>();
        // down
        col.size = new Vector2(border.width - 1.0f, 1.0f);
        col.offset = new Vector2(border.x - 1.0f, border.y - 1.0f);

        // right
        col = gameObject.AddComponent<BoxCollider2D>();
        col.size = new Vector2(1.0f, border.height + 2.0f);
        col.offset = new Vector2(border.x + border.width, border.y - 1.0f);

        // up
        col = gameObject.AddComponent<BoxCollider2D>();
        col.size = new Vector2(border.width + 2.0f, 1.0f);
        col.offset = new Vector2(border.x - 1.0f, border.y + border.height);

        // left
        col = gameObject.AddComponent<BoxCollider2D>();
        col.size = new Vector2(1.0f, border.height + 2.0f);
        col.offset = new Vector2(border.x - 1.0f, border.y - 1.0f);

        BoxCollider2D[] colls = gameObject.GetComponents<BoxCollider2D>();
        for (int i = 0; i < colls.Length; i++)
        {
            colls[i].offset += colls[i].size * 0.5f;
        }

        Tilemap map = GameObject.Find("TilemapFloor").GetComponent<Tilemap>();
        TileBase tile = tileset.tileFloor;
        int xSize = Mathf.FloorToInt(border.width), ySize = Mathf.FloorToInt(border.height), xPos = Mathf.FloorToInt(border.x), yPos = Mathf.FloorToInt(border.y);

        List<Vector2Int> walkableTiles = new List<Vector2Int>();
        for (int x = xPos; x < xPos + xSize; x++)
        {
            for (int y = yPos; y < yPos + ySize; y++)
            {
                walkableTiles.Add(new Vector2Int(x, y));
                map.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }

        room.walkableTiles = walkableTiles;

        Destroy(this);
    }
}
