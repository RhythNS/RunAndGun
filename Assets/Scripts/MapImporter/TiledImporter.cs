using System.IO;
using TiledSharp;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TiledImporter
{
    public static TmxMap LoadMap(string mapName)
    {
        TextAsset mapString = TiledDict.Instance.GetTileMap(mapName).tsmFile;
        MemoryStream stream = new MemoryStream(mapString.bytes);
        TmxMap map = new TmxMap(stream);
        stream.Close();
        return map;
    }

    /// <summary>
    /// Replaces a section of the tilemap with a TiledMap.
    /// </summary>
    /// <param name="tileMap">The tilemap to be edited.</param>
    /// <param name="mapName">The name of the TiledMap.</param>
    /// <param name="x">The starting x point where the TiledMap will be placed to.</param>
    /// <param name="y">The starting y point where the TiledMap will be placed to.</param>
    public static void ReplaceSection(Tilemap tileMap, string mapName, int x, int y)
    {
        TmxMap map = LoadMap(mapName);

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

                // Get the correct tileset with the gid value
                TmxTileset toFindTileset = null;
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
                gid += toFindTileset.TileCount ?? 0;

                Texture2D tilesetImage = toFindTileset.Image;

                int tileWidth = toFindTileset.TileWidth;
                int tileHeight = toFindTileset.TileHeight;

                int columns = toFindTileset.Columns.Value;

                int column = gid % columns;
                int row = gid / columns;

                /*
                int orgX = layerTile.X * tileWidth;
                int orgY = layerTile.Y * tileHeight;

                if (toFindTileset.Tiles.TryGetValue(gid, out TmxTilesetTile tilesetTile) == true)
                {
                    // Get all hitboxes
                    for (int groupIndex = 0; groupIndex < tilesetTile.ObjectGroups.Count; groupIndex++)
                    {
                        for (int objectIndex = 0; objectIndex < tilesetTile.ObjectGroups[groupIndex].Objects.Count; objectIndex++)
                        {
                            tiledBase.NotifyHitboxLoaded(mapComponent, transform, tilesetTile.ObjectGroups[groupIndex].Objects[objectIndex], orgX, orgY);
                        }
                    }
                }
                */
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

                tileMap.SetTile(new Vector3Int(layerTile.X + x, map.Height - layerTile.Y + y, l), tile);
            }
        }

        for (int i = 0; i < map.ObjectGroups.Count; i++)
        {
            // TODO: Load the objects
        }
    }
}
