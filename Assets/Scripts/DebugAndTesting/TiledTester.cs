using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Testing for loading a map from Tiled.
/// </summary>
public class TiledTester : MonoBehaviour
{
    [SerializeField] private string toLoad;
    [SerializeField] int x;
    [SerializeField] int y;
    [SerializeField] private Tilemap tilemap;

    private void Start()
    {
        TiledImporter.Instance.ReplaceSection(tilemap, toLoad, x, y, out _);

        var v = TiledImporter.Instance.GetReplacableMap("room1", out _, out _);
    }

}
