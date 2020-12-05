using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonCreator : MonoBehaviour
{
    [HideInInspector]
    public Dungeon dungeon;

    [SerializeField]
    private Dungeon.DungeonTileset tileset;

    [SerializeField]
    private Vector2Int maxSize = Vector2Int.one;

    public void CreateDungeon() {
        if (dungeon != null) {
            dungeon.Reset();
        }

        int childrenCount = transform.childCount;
        for (int i = childrenCount - 1; i >= 0; i--) {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        dungeon = new Dungeon(maxSize);

        for (int y = 0; y < maxSize.y; y++) {
            for (int x = 1; x < maxSize.x - 1; x++) {
                if (dungeon.Map[x, y] == Dungeon.TileTypes.Tile) {
                    //SpawnTile(x, y);

                    GameObject obj = Instantiate(tileset.tileStraight, transform.position + new Vector3(x, y, 0), Quaternion.Euler(0, 0, 0));
                    obj.transform.parent = this.transform;
                }
            }
        }
    }

    private Dungeon.TileTypes GetTile(int x, int y) {
        if (x < 0 || x >= dungeon.Map.GetLength(0) || y < 0 || y >= dungeon.Map.GetLength(1)) {
            return Dungeon.TileTypes.Empty;
        }

        return dungeon.Map[x, y];
    }

    private void SpawnTile(int x, int y) {
        GameObject obj = null;

        if (GetTile(x - 1, y) != Dungeon.TileTypes.Tile
            && GetTile(x + 1, y) != Dungeon.TileTypes.Tile
            && GetTile(x, y - 1) == Dungeon.TileTypes.Tile
            && GetTile(x, y + 1) == Dungeon.TileTypes.Tile) {

            obj = Instantiate(tileset.tileStraight, transform.position + new Vector3(x, 0, y) * 2.5f, Quaternion.Euler(0, 0, 0));
        } else if (dungeon.Map[x - 1, y] == Dungeon.TileTypes.Tile
              && dungeon.Map[x + 1, y] == Dungeon.TileTypes.Tile
              && dungeon.Map[x, y - 1] != Dungeon.TileTypes.Tile
              && dungeon.Map[x, y + 1] != Dungeon.TileTypes.Tile) {

            obj = Instantiate(tileset.tileStraight, transform.position + new Vector3(x, 0, y) * 2.5f, Quaternion.Euler(0, 90, 0));
        } else if (dungeon.Map[x - 1, y] == Dungeon.TileTypes.Tile
              && dungeon.Map[x, y + 1] == Dungeon.TileTypes.Tile
              && dungeon.Map[x, y - 1] != Dungeon.TileTypes.Tile
              && dungeon.Map[x + 1, y] != Dungeon.TileTypes.Tile) {

            obj = Instantiate(tileset.tileCurve, transform.position + new Vector3(x, 0, y) * 2.5f, Quaternion.Euler(0, -90, 0));
        } else if (dungeon.Map[x + 1, y] == Dungeon.TileTypes.Tile
              && dungeon.Map[x, y - 1] == Dungeon.TileTypes.Tile
              && dungeon.Map[x, y + 1] != Dungeon.TileTypes.Tile
              && dungeon.Map[x - 1, y] != Dungeon.TileTypes.Tile) {

            obj = Instantiate(tileset.tileCurve, transform.position + new Vector3(x, 0, y) * 2.5f, Quaternion.Euler(0, 90, 0));
        } else if (dungeon.Map[x - 1, y] == Dungeon.TileTypes.Tile
              && dungeon.Map[x, y - 1] == Dungeon.TileTypes.Tile
              && dungeon.Map[x, y + 1] != Dungeon.TileTypes.Tile
              && dungeon.Map[x + 1, y] != Dungeon.TileTypes.Tile) {

            obj = Instantiate(tileset.tileCurve, transform.position + new Vector3(x, 0, y) * 2.5f, Quaternion.Euler(0, 180, 0));
        } else if (dungeon.Map[x + 1, y] == Dungeon.TileTypes.Tile
              && dungeon.Map[x, y + 1] == Dungeon.TileTypes.Tile
              && dungeon.Map[x, y - 1] != Dungeon.TileTypes.Tile
              && dungeon.Map[x - 1, y] != Dungeon.TileTypes.Tile) {

            obj = Instantiate(tileset.tileCurve, transform.position + new Vector3(x, 0, y) * 2.5f, Quaternion.Euler(0, 0, 0));
        } else if (dungeon.Map[x + 1, y] == Dungeon.TileTypes.Tile
              && dungeon.Map[x, y + 1] == Dungeon.TileTypes.Tile
              && dungeon.Map[x, y - 1] == Dungeon.TileTypes.Tile
              && dungeon.Map[x - 1, y] == Dungeon.TileTypes.Tile) {

            obj = Instantiate(tileset.tileJunction, transform.position + new Vector3(x, 0, y) * 2.5f, Quaternion.Euler(0, 0, 0));
        } else if (dungeon.Map[x + 1, y] == Dungeon.TileTypes.Tile
              && dungeon.Map[x, y + 1] != Dungeon.TileTypes.Tile
              && dungeon.Map[x, y - 1] != Dungeon.TileTypes.Tile
              && dungeon.Map[x - 1, y] != Dungeon.TileTypes.Tile) {

            obj = Instantiate(tileset.tileDeadend, transform.position + new Vector3(x, 0, y) * 2.5f, Quaternion.Euler(0, 90, 0));
        } else if (dungeon.Map[x - 1, y] == Dungeon.TileTypes.Tile
              && dungeon.Map[x, y + 1] != Dungeon.TileTypes.Tile
              && dungeon.Map[x, y - 1] != Dungeon.TileTypes.Tile
              && dungeon.Map[x + 1, y] != Dungeon.TileTypes.Tile) {

            obj = Instantiate(tileset.tileDeadend, transform.position + new Vector3(x, 0, y) * 2.5f, Quaternion.Euler(0, -90, 0));
        } else if (dungeon.Map[x, y + 1] == Dungeon.TileTypes.Tile
               && dungeon.Map[x + 1, y] != Dungeon.TileTypes.Tile
               && dungeon.Map[x, y - 1] != Dungeon.TileTypes.Tile
               && dungeon.Map[x - 1, y] != Dungeon.TileTypes.Tile) {

            obj = Instantiate(tileset.tileDeadend, transform.position + new Vector3(x, 0, y) * 2.5f, Quaternion.Euler(0, 0, 0));
        } else if (dungeon.Map[x, y - 1] == Dungeon.TileTypes.Tile
               && dungeon.Map[x, y + 1] != Dungeon.TileTypes.Tile
               && dungeon.Map[x + 1, y] != Dungeon.TileTypes.Tile
               && dungeon.Map[x - 1, y] != Dungeon.TileTypes.Tile) {

            obj = Instantiate(tileset.tileDeadend, transform.position + new Vector3(x, 0, y) * 2.5f, Quaternion.Euler(0, 180, 0));
        } else if (dungeon.Map[x + 1, y] == Dungeon.TileTypes.Tile
              && dungeon.Map[x, y + 1] == Dungeon.TileTypes.Tile
              && dungeon.Map[x, y - 1] == Dungeon.TileTypes.Tile
              && dungeon.Map[x - 1, y] != Dungeon.TileTypes.Tile) {

            obj = Instantiate(tileset.tileTJunction, transform.position + new Vector3(x, 0, y) * 2.5f, Quaternion.Euler(0, 90, 0));
        } else if (dungeon.Map[x - 1, y] == Dungeon.TileTypes.Tile
             && dungeon.Map[x, y + 1] == Dungeon.TileTypes.Tile
             && dungeon.Map[x, y - 1] == Dungeon.TileTypes.Tile
             && dungeon.Map[x + 1, y] != Dungeon.TileTypes.Tile) {

            obj = Instantiate(tileset.tileTJunction, transform.position + new Vector3(x, 0, y) * 2.5f, Quaternion.Euler(0, -90, 0));
        } else if (dungeon.Map[x + 1, y] == Dungeon.TileTypes.Tile
             && dungeon.Map[x - 1, y] == Dungeon.TileTypes.Tile
             && dungeon.Map[x, y - 1] == Dungeon.TileTypes.Tile
             && dungeon.Map[x, y + 1] != Dungeon.TileTypes.Tile) {

            obj = Instantiate(tileset.tileTJunction, transform.position + new Vector3(x, 0, y) * 2.5f, Quaternion.Euler(0, 180, 0));
        } else if (dungeon.Map[x + 1, y] == Dungeon.TileTypes.Tile
             && dungeon.Map[x, y + 1] == Dungeon.TileTypes.Tile
             && dungeon.Map[x - 1, y] == Dungeon.TileTypes.Tile
             && dungeon.Map[x, y - 1] != Dungeon.TileTypes.Tile) {

            obj = Instantiate(tileset.tileTJunction, transform.position + new Vector3(x, 0, y) * 2.5f, Quaternion.Euler(0, 0, 0));
        }

        if (obj != null) {
            obj.transform.parent = this.transform;
            obj.transform.localScale = Vector3.one * 2.5f;
        }
    }

    private void OnDestroy() {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
    }
}
