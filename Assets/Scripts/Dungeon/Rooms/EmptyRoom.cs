using MapGenerator;

public class EmptyRoom : DungeonRoom
{
    public override bool EventOnRoomEntered => false;

    public override RoomType RoomType => RoomType.Empty;
}
