using MapGenerator;
using UnityEngine;

/// <summary>
/// The first of room of the dungeon where the players are placed on first.
/// </summary>
public class StartRoom : DungeonRoom
{
    public override bool EventOnRoomEntered => false;

    public override RoomType RoomType => RoomType.Start;

    /// <summary>
    /// Spawn starting items.
    /// </summary>
    public void SpawnItems(Pickable[] pickables, Vector3 vector3)
    {
        for (int i = 0; i < pickables.Length; i++)
        {
            Vector3 pos = vector3 + new Vector3(1.0f * (i - pickables.Length / 2.0f), 0.0f, 0.0f);
            //PickableInWorld.Place(pickables[i], MathUtil.RandomVector3(new Vector3(-1.0f, -1.0f), new Vector3(1.0f, 1.0f)) + vector3);
            PickableInWorld.Place(pickables[i], pos);
        }
    }
}
