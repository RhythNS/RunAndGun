using UnityEngine;

[CreateAssetMenu(fileName = "Tileset", menuName = "MapGenerator/Tileset", order = 1)]
public class Tileset : ScriptableObject
{
    /// <summary>
    /// The tile to use for the floor.
    /// </summary>
    public RuleTile tileFloor;

    /// <summary>
    /// The tile to use for the walls.
    /// </summary>
    public RuleTile tileWall;

    /// <summary>
    /// The tile to use for the ceiling.
    /// </summary>
    public RuleTile tileCeiling;
}
