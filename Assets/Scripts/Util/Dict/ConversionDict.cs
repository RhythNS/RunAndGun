using UnityEngine;

public class ConversionDict : MonoBehaviour
{
    public static ConversionDict Instance { get; private set; }

    [HideInInspector] public Vector3 offsetPosition;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Already a ConversionDict in scene! Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Calculates the world position from a tile and adding half.
    /// </summary>
    public static Vector3 TilePositionToWorldPositionMiddle(Vector2Int pos)
    {
        return Instance.offsetPosition + new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0.0f);
        //return tilemapFloor.CellToWorld((Vector3Int)pos) + tilemapFloor.cellSize * 0.5f;
    }

    /// <summary>
    /// Calculates the world position from a tile and adding half.
    /// </summary>
    public static Vector3 TilePositionToWorldPositionMiddle(int x, int y)
    {
        return Instance.offsetPosition + new Vector3(x + 0.5f, y + 0.5f, 0.0f);
        // return tilemapFloor.CellToWorld(new Vector3Int(x, y, 0)) + tilemapFloor.cellSize * 0.5f;
    }

    /// <summary>
    /// Calculates the world position from a tile.
    /// </summary>
    public static Vector3 TilePositionToWorldPosition(Vector2Int pos)
    {
        return Instance.offsetPosition + new Vector3(pos.x, pos.y, 0.0f);
        // return tilemapFloor.CellToWorld((Vector3Int)pos);
    }

    /// <summary>
    /// Calculates the tile position from a worldposition.
    /// </summary>
    public static Vector2Int WorldPositionToTilePosition(Vector3 pos)
    {
        return new Vector2Int((int)(pos.x - Instance.offsetPosition.x), (int)(pos.y - Instance.offsetPosition.y));
        // return (Vector2Int)tilemapFloor.WorldToCell(pos);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
