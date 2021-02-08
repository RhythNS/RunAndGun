using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CreateColliderFromRoom : MonoBehaviour
{
    [SerializeField] private Tileset tileset;

    private void Start()
    {
        DungeonRoom room = GetComponent<DungeonRoom>();

        Tilemap map = GameObject.Find("TilemapFloor").GetComponent<Tilemap>();
        TileBase tile = tileset.tileFloor;

        Rect border = room.Border;
        int xSize = Mathf.FloorToInt(border.width), ySize = Mathf.FloorToInt(border.height), xPos = Mathf.FloorToInt(border.x), yPos = Mathf.FloorToInt(border.y);

        List<Vector2Int> walkableTiles = new List<Vector2Int>();
        for (int x = xPos; x < xPos + xSize; x++)
        {
            for (int y = yPos; y < yPos + ySize; y++)
            {
                RaycastHit2D hit = Physics2D.BoxCast(new Vector2(x + 0.5f, y + 0.5f), new Vector2(1, 1), 0, new Vector2(0, 0), 0.0f);
                if (hit.collider == null)
                    walkableTiles.Add(new Vector2Int(x, y));
                map.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }

        room.walkableTiles = walkableTiles;
        DebugPathFinder.Instance.SetRoom(xSize, ySize, xPos, yPos, walkableTiles);

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
    }

    private void OnDrawGizmosSelected()
    {
        DungeonRoom room = GetComponent<DungeonRoom>();
        for (int i = 0; i < room.walkableTiles.Count; i++)
        {
            Gizmos.color = Color.white;
            Vector2Int pos = room.walkableTiles[i];
            Gizmos.DrawWireCube(new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0), new Vector3(1, 1, 1));
        }
    }
}
