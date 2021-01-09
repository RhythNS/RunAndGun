using UnityEngine;
using UnityEngine.Tilemaps;

public class TiledTester : MonoBehaviour
{
    [SerializeField] private string toLoad;
    [SerializeField] int x;
    [SerializeField] int y;
    [SerializeField] private Tilemap tilemap;

    private void Start()
    {
        TiledImporter.Instance.ReplaceSection(tilemap, toLoad, x, y, out _);

        var v = TiledImporter.Instance.GetReplacableMap("room1", x, y, out _);
    }

}
