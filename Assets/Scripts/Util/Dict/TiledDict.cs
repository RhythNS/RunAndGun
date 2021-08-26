using System.Collections.Generic;
using TiledSharp;
using UnityEngine;

/// <summary>
/// Dict for tiled data.
/// </summary>
public class TiledDict : MonoBehaviour
{
    public static TiledDict Instance { get; private set; }

    [SerializeField] private Tilemap[] tileMaps;
    [SerializeField] private Tileset[] tilesets;
    [SerializeField] private CustomObject[] customObjects;

    private readonly Dictionary<string, Tileset> tilesetDict = new Dictionary<string, Tileset>();
    private readonly Dictionary<string, Tilemap> mapDict = new Dictionary<string, Tilemap>();
    private readonly Dictionary<string, GameObject> objectDict = new Dictionary<string, GameObject>();

    public int TileMapCount { get => tileMaps.Length; }

    [System.Serializable]
    public struct Tileset
    {
        public Texture2D image;
        public string name;
        public TextAsset tsxFile;
        public TmxTileset tileset;
    }

    [System.Serializable]
    public struct Tilemap
    {
        public string name;
        public TextAsset tsmFile;
    }

    [System.Serializable]
    public struct CustomObject
    {
        public string name;
        public GameObject prefab;
    }

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogError("TiledDict already in scene! Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;

        // move all arrays into dictionaries for quicker access.

        for (int i = 0; i < tilesets.Length; i++)
        {
            tilesets[i].tileset = new TmxTileset(tilesets[i].tsxFile)
            {
                Image = tilesets[i].image
            };
            tilesetDict.Add(tilesets[i].name, tilesets[i]);
        }

        for (int i = 0; i < tileMaps.Length; i++)
        {
            mapDict.Add(tileMaps[i].name, tileMaps[i]);
        }

        for (int i = 0; i < customObjects.Length; i++)
        {
            objectDict.Add(customObjects[i].name, customObjects[i].prefab);
        }
    }

    /// <summary>
    /// Gets a specified tileset.
    /// </summary>
    /// <param name="name">The name of the tileset.</param>
    /// <returns>A reference to the tileset.</returns>
    public Tileset GetTileset(string name)
    {
        return tilesetDict[name];
    }

    /// <summary>
    /// Gets a specified tilemap.
    /// </summary>
    /// <param name="name">The name of the tilemap.</param>
    /// <returns>A reference to the tilemap.</returns>
    public Tilemap GetTileMap(string name)
    {
        return mapDict[name];
    }

    /// <summary>
    /// Try to get a custom object.
    /// </summary>
    /// <param name="name">The name of the custom object.</param>
    /// <param name="obj">A reference to the object if found.</param>
    /// <returns>Wheter it found the object or not.</returns>
    public bool TryGetObject(string name, out GameObject obj)
    {
        if (objectDict.TryGetValue(name, out obj))
            return true;
        Debug.LogWarning("Could not find CustomObject " + name + "! I am not spawning him onto the map!");
        return false;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
