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
        TiledImporter.ReplaceSection(tilemap, toLoad, x, y);
    }

}
