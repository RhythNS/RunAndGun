using MapGenerator;
using UnityEngine;

/// <summary>
/// A room where players can buy items with their money.
/// </summary>
public class ShopRoom : DungeonRoom
{
    public Pickable[] shopItems;
    public uint[] prices;
    public Vector2[] locations;

    public override bool EventOnRoomEntered => false;

    public override RoomType RoomType => RoomType.Shop;

    /// <summary>
    /// Spawns the itmes that the playes can buy.
    /// </summary>
    public void SpawnItems()
    {
        if (Player.LocalPlayer.isServer == false)
            return;

        for (int i = 0; i < shopItems.Length; i++)
        {
            PickableInWorld.Place(shopItems[i], locations[i], true);
        }
    }
}
