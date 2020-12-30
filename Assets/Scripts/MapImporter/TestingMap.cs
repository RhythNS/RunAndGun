using System.Collections.Generic;
using System.IO;
using TiledSharp;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestingMap : MonoBehaviour
{
    public Tilemap tileMap;
    public string location;
    public string mapName;
    private List<Texture2D> texs = new List<Texture2D>();

    private Tile[] tiles;

    private void Start()
    {
        TextAsset mapString = Resources.Load(location + "/" + mapName) as TextAsset;
        MemoryStream stream = new MemoryStream(mapString.bytes);
        TmxMap map = new TmxMap(stream);
        stream.Close();

        for (int i = 0; i < map.Tilesets.Count; i++)
        {
            //map.Tilesets[i].Load(location);
            string image = map.Tilesets[i].Image.Source;
            image = image.Substring(0, image.Length - 4);
            texs.Add(Resources.Load<Texture2D>(location + "/" + image));
        }

        tiles = new Tile[map.Tilesets[0].TileCount.Value];
        Rect rect = new Rect(0.0f, 0.0f, map.TileWidth, map.TileHeight);
        int columns = map.Tilesets[0].Columns.Value;
        for (int i = 0; i < map.Tilesets[0].TileCount.Value; i++)
        {
            tiles[i] = ScriptableObject.CreateInstance<Tile>();
            int column = i % columns;
            int row = i / columns;

            rect.position = new Vector2(column * map.TileWidth, texs[0].height - (row + 1) * map.TileHeight);
            tiles[i].sprite = Sprite.Create(texs[0], rect, new Vector2(), map.Tilesets[0].TileWidth);
        }

        for (int l = 0; l < map.TileLayers.Count; l++)
        {
            for (int t = 0; t < map.TileLayers[l].Tiles.Count; t++)
            {
                TmxLayerTile layerTile = map.TileLayers[l].Tiles[t];
                int gid = layerTile.Gid;

                // Empty tile, do nothing
                if (gid == 0)
                    continue;

                // Get the acctual gid value
                gid--;

                Tile tile = tiles[gid];

                tileMap.SetTile(new Vector3Int(layerTile.X, map.Height - layerTile.Y, l), tile);
            }
        }
    }

}
