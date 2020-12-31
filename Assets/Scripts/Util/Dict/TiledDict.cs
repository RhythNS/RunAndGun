using System.Collections.Generic;
using TiledSharp;
using UnityEngine;

public class TiledDict : MonoBehaviour
{
    public static TiledDict Instance { get; private set; }

    [SerializeField] private Tilemap[] tileMaps;
    [SerializeField] private Tileset[] tilesets;

    private readonly Dictionary<string, Tileset> tilesetDict = new Dictionary<string, Tileset>();
    private readonly Dictionary<string, Tilemap> mapDict = new Dictionary<string, Tilemap>();

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

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogError("TiledDict already in scene! Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;

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
    }

    public Tileset GetTileset(string name)
    {
        return tilesetDict[name];
    }

    public Tilemap GetTileMap(string name)
    {
        return mapDict[name];
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
