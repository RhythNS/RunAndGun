using MapGenerator;

/// <summary>
/// An empty room.
/// </summary>
public class EmptyRoom : DungeonRoom
{
    public override bool EventOnRoomEntered => false;

    public override RoomType RoomType => RoomType.Empty;
}
