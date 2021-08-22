using MapGenerator;

/// <summary>
/// A room that spawns loot when all players entered.
/// </summary>
public class LootRoom : DungeonRoom
{
    public override bool EventOnRoomEntered => true;

    public override RoomType RoomType => RoomType.Loot;

    public Pickable[] pickables;

    public override void OnAllPlayersEntered()
    {
        SpawnLoot(pickables);
        AlreadyCleared = true;
    }
}
