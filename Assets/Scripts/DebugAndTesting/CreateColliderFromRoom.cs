using UnityEngine;

public class CreateColliderFromRoom : MonoBehaviour
{
    private void Start()
    {
        Rect border = GetComponent<DungeonRoom>().Border;


        BoxCollider2D col;
        col = gameObject.AddComponent<BoxCollider2D>();
        // down
        col.size = new Vector2(border.width, 1.0f);
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

        Destroy(this);
    }
}
