using MapGenerator;
using UnityEngine;

/// <summary>
/// A room where players can buy items with their money.
/// </summary>
public class ShopRoom : DungeonRoom
{
    public Pickable[] shopItems;
    public Vector2[] locations;
    public GameObject[] itemPriceSigns;

    public override bool EventOnRoomEntered => false;

    public override RoomType RoomType => RoomType.Shop;

    /// <summary>
    /// Spawns the itmes that the playes can buy.
    /// </summary>
    public void SpawnItems()
    {
        if (Player.LocalPlayer.isServer == false)
            return;

        itemPriceSigns = new GameObject[shopItems.Length];

        for (int i = 0; i < shopItems.Length; i++)
        {
            itemPriceSigns[i] = Instantiate(RegionDict.Instance.ShopItemPriceSign, locations[i] + Vector2.up * 1.5f, Quaternion.identity);
            ShopItemPriceDisplay sipd = itemPriceSigns[i].GetComponent<ShopItemPriceDisplay>();
            sipd.SetPrice(shopItems[i].Costs);
            Mirror.NetworkServer.Spawn(itemPriceSigns[i]);

            PickableInWorld.Place(shopItems[i], locations[i], true);
        }
    }
}
