using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TiledSharp;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TiledImporter : MonoBehaviour
{
    public static TiledImporter Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("TiledImporter already in scene! Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Loads a tiled map based on its name.
    /// </summary>
    /// <param name="mapName">The name of the map.</param>
    /// <returns>The loaded tiled map.</returns>
    public TmxMap LoadMap(string mapName)
    {
        TextAsset mapString = TiledDict.Instance.GetTileMap(mapName).tsmFile;
        MemoryStream stream = new MemoryStream(mapString.bytes);
        TmxMap map = new TmxMap(stream);
        stream.Close();
        return map;
    }

    /// <summary>
    /// Loads all objects from all object layers from a map.
    /// </summary>
    /// <param name="map">The map where the objects are on.</param>
    /// <param name="x">The x offset of the map.</param>
    /// <param name="y">The y offset of the map.</param>
    /// <param name="loadedGameobjects">A list of all the loaded objects.</param>
    private void LoadAllObjects(TmxMap map, int x, int y, List<GameObject> loadedGameobjects)
    {
        // Go through each objectlayer and spawn their objects
        for (int i = 0; i < map.ObjectGroups.Count; i++)
        {
            for (int j = 0; j < map.ObjectGroups[i].Objects.Count; j++)
            {
                if (LoadObject(map, map.ObjectGroups[i].Objects[j], x, y, out GameObject obj))
                    loadedGameobjects.Add(obj);
            }
        }
    }


    /// <summary>
    /// Instantiates a gameobject based on a tiled object.
    /// </summary>
    /// <param name="map">The map where the tiled object is from.</param>
    /// <param name="tmxObject">The object that was loaded.</param>
    /// <param name="xOffset">The x offset of the map.</param>
    /// <param name="yOffset">The y offset of the map.</param>
    /// <param name="obj">The loaded gameobject.</param>
    /// <returns>Wheter it succeeded ot not.</returns>
    private bool LoadObject(TmxMap map, TmxObject tmxObject, float xOffset, float yOffset, out GameObject obj)
    {
        // Try to get the prefab
        if (TiledDict.Instance.TryGetObject(tmxObject.Type, out obj) == false)
            return false;

        // Instantiate and position the object
        obj = Instantiate(obj);
        Vector3 pos = obj.transform.position;
        pos.x = xOffset + ((float)tmxObject.X / map.TileWidth);
        pos.y = yOffset + map.Height - ((float)tmxObject.Y / map.TileHeight);
        obj.transform.position = pos;

        // if there are no custom properties, we are done.
        if (tmxObject.Properties.Count == 0)
            return true;

        ICustomTiledObject customObject = obj.GetComponent<ICustomTiledObject>();
        Type type = customObject.GetType();
        // Go over each custom property
        foreach (string key in tmxObject.Properties.Keys)
        {
            // Get the reflection field info
            FieldInfo fieldInfo = type.GetField(key);
            if (fieldInfo == null)
            {
                Debug.LogWarning("Could not find field " + key + " on custom object " + tmxObject.Type + "!");
                continue;
            }

            // Assign the field the value that we set in tiled.
            // This does not look pretty, but I could not think of another way to do this.
            Type fieldType = fieldInfo.FieldType;
            if (fieldType == typeof(int))
                fieldInfo.SetValue(customObject, int.Parse(tmxObject.Properties[key]));
            else if (fieldType == typeof(string))
                fieldInfo.SetValue(customObject, tmxObject.Properties[key]);
            else if (fieldType == typeof(bool))
                fieldInfo.SetValue(customObject, bool.Parse(tmxObject.Properties[key]));
            else if (fieldType == typeof(float))
                fieldInfo.SetValue(customObject, float.Parse(tmxObject.Properties[key]));
            else
                fieldInfo.SetValue(customObject, tmxObject.Properties[key]);
        }

        return true;
    }

    /// <summary>
    /// Replaces a section of the tilemap with a TiledMap.
    /// </summary>
    /// <param name="tileMap">The tilemap to be edited.</param>
    /// <param name="mapName">The name of the TiledMap.</param>
    /// <param name="x">The starting x point where the TiledMap will be placed to.</param>
    /// <param name="y">The starting y point where the TiledMap will be placed to.</param>
    public void ReplaceSection(Tilemap tileMap, string mapName, int x, int y, out List<GameObject> loadedGameobjects)
    {
        TmxMap map = LoadMap(mapName);
        loadedGameobjects = new List<GameObject>();

        for (int l = 0; l < map.TileLayers.Count; l++)
        {
            for (int t = 0; t < map.TileLayers[l].Tiles.Count; t++)
            {
                TmxLayerTile layerTile = map.TileLayers[l].Tiles[t];

                int gid = GetGid(map, layerTile, out TmxTileset toFindTileset);
                if (gid == -1)
                    continue;

                Texture2D tilesetImage = toFindTileset.Image;

                int tileWidth = toFindTileset.TileWidth;
                int tileHeight = toFindTileset.TileHeight;

                int columns = toFindTileset.Columns.Value;

                int column = gid % columns;
                int row = gid / columns;

                // If the tileset tile has objects, spawn them
                /*
                if (toFindTileset.Tiles.TryGetValue(gid, out TmxTilesetTile tilesetTile) == true)
                {
                    for (int groupIndex = 0; groupIndex < tilesetTile.ObjectGroups.Count; groupIndex++)
                    {
                        for (int objectIndex = 0; objectIndex < tilesetTile.ObjectGroups[groupIndex].Objects.Count; objectIndex++)
                        {
                            if (LoadObject(map, tilesetTile.ObjectGroups[groupIndex].Objects[objectIndex], x, y, out GameObject obj))
                                loadedGameobjects.Add(obj);
                        }
                    }
                }
                 */

                // Create and set the tile on the tilemap
                Tile tile = ScriptableObject.CreateInstance<Tile>();
                tile.sprite = Sprite.Create(
                    tilesetImage,
                    new Rect(
                        column * map.TileWidth,
                        tilesetImage.height - (row + 1) * map.TileHeight,
                        tileWidth,
                        tileHeight),
                    new Vector2(),
                    tileHeight
                    );

                tileMap.SetTile(new Vector3Int(layerTile.X + x, map.Height - layerTile.Y - 1 + y, l), tile);
            }
        }

        // Load objects from the maps objectgroups
        LoadAllObjects(map, x, y, loadedGameobjects);
    }

    /// <summary>
    /// Container for loaded TiledObjects.
    /// </summary>
    public struct PrefabLocations
    {
        public GameObject Prefab { get; set; }
        /// <summary>
        /// Position in local space
        /// </summary>
        public Vector3 Position { get; set; }
    }

    /// <summary>
    /// Gets the gid values of the tiles in an array. The gid are not the acctual gid values of the map, but the gid
    /// values of the tileset. Assume tileset a has 10 tiles and tileset b has 10 tiles. If at position 1 there is
    /// the tileset tile 1 of tileset a and at position 2 the tileset tile 1 of tileset b then the gid values of the
    /// return array are the same. To put it simply, just use one tileset and one layer for this method.
    /// </summary>
    /// <param name="mapName">The name of the map to get the gid values from.</param>
    /// <param name="prefabs">A list of all prefabs and their local position.</param>
    /// <returns>The gid values of the map.</returns>
    public Fast2DArray<int> GetReplacableMap(string mapName, out PropertyDict properties, out List<PrefabLocations> prefabs)
    {
        prefabs = new List<PrefabLocations>();

        TmxMap map = LoadMap(mapName);
        properties = map.Properties;

        Fast2DArray<int> retArr = new Fast2DArray<int>(map.Width, map.Height);

        if (map.TileLayers.Count == 0)
            throw new Exception("Replacable map " + mapName + " has no tile layers!");
        else if (map.TileLayers.Count > 1)
            Debug.LogWarning("Replacable map " + mapName + " has more than 1 tile layer. I am only reading the first layer!");
        if (map.Tilesets.Count != 1)
            Debug.LogWarning("Replacable map " + mapName + " has not only 1 tileset. This might cause issues!");

        for (int t = 0; t < map.TileLayers[0].Tiles.Count; t++)
        {
            TmxLayerTile layerTile = map.TileLayers[0].Tiles[t];
            int gid = GetGid(map, layerTile, out _);
            retArr.Set(gid, layerTile.X, map.Height - layerTile.Y - 1);
        }

        //LoadAllObjects(map, atX, atY, loadedGameobjects);

        // Go through each objectlayer and spawn their objects
        for (int i = 0; i < map.ObjectGroups.Count; i++) {
            for (int j = 0; j < map.ObjectGroups[i].Objects.Count; j++) {
                // Try to get the prefab
                if (TiledDict.Instance.TryGetObject(map.ObjectGroups[i].Objects[j].Type, out GameObject obj) == false)
                    continue;

                // Instantiate and position the object
                Vector3 pos;
                pos.x = ((float)map.ObjectGroups[i].Objects[j].X / map.TileWidth);
                pos.y = map.Height - ((float)map.ObjectGroups[i].Objects[j].Y / map.TileHeight);
                pos.z = 0f;

                prefabs.Add(new PrefabLocations { Prefab = obj, Position = pos });
            }
        }

        return retArr;
    }

    /// <summary>
    /// Gets the acctual gid of a TilesetTile and returns the tileset that the tile is from-
    /// </summary>
    /// <param name="map">The map where the TilesetTile is from.</param>
    /// <param name="layerTile">The TilesetTile to find the correct gid from.</param>
    /// <param name="toFindTileset">The correct Tileset.</param>
    /// <returns>The gid value.</returns>
    private int GetGid(TmxMap map, TmxLayerTile layerTile, out TmxTileset toFindTileset)
    {
        int gid = layerTile.Gid;
        toFindTileset = null;

        // Empty tile, do nothing
        if (gid == 0)
            return -1;

        // Get the acctual gid value
        gid--;

        // Get the correct tileset with the gid value
        for (int j = 0; j < map.Tilesets.Count; j++)
        {
            gid -= map.Tilesets[j].TileCount ?? 0;
            if (gid <= 0)
            {
                toFindTileset = map.Tilesets[j];
                break;
            }
        }

        // set gid to a positive value again
        return gid + toFindTileset.TileCount ?? 0;
    }


    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
