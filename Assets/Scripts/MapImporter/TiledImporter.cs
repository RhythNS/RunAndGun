using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TiledImporter : MonoBehaviour
{
    /// <summary>
    /// Replaces a section of the tilemap with a TiledMap.
    /// </summary>
    /// <param name="tilemap">The tilemap to be edited.</param>
    /// <param name="mapName">The name of the TiledMap.</param>
    /// <param name="x">The starting x point where the TiledMap will be placed to.</param>
    /// <param name="y">The starting y point where the TiledMap will be placed to.</param>
    /// <returns>Wheter it succeeded.</returns>
    public bool ReplaceSection(Tilemap tilemap, string mapName, int x, int y)
    {
        // TODO: Implement
        return false;
    }
}
