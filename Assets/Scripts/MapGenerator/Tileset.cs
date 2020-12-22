using UnityEngine;

[CreateAssetMenu(fileName = "Tileset", menuName = "MapGenerator/Tileset", order = 1)]
class Tileset : ScriptableObject
{
    public RuleTile tileFloor;
    public RuleTile tileWall;
    public RuleTile tileCeiling;
}
