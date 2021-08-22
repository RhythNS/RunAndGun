using MapGenerator;
using UnityEngine;

/// <summary>
/// Corridors connect other rooms with each other.
/// </summary>
public class CorridorRoom : DungeonRoom
{
    public override bool EventOnRoomEntered => false;

    public override RoomType RoomType => RoomType.Corridor;

    /// <summary>
    /// The direction of the corridor.
    /// true = horizontal, false = vertical
    /// </summary>
    public bool direction;

    public override void OnLocalPlayerEntered()
    {
        if (direction)
            DungeonCreator.Instance.AdjustMask(new Vector2(Border.xMin - 1.5f, Border.yMin), new Vector2(Border.size.x + 3, Border.size.y + 2));
        else
            DungeonCreator.Instance.AdjustMask(new Vector2(Border.xMin - 0.5f, Border.yMin - 2.5f), new Vector2(Border.size.x + 1, Border.size.y + 6));
    }
}
