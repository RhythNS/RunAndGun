using MapGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ShopRoom : DungeonRoom
{
    public Pickable[] shopItems;
    public uint[] prices;
    public Vector2[] locations;

    public override bool EventOnRoomEntered => false;

    public override RoomType RoomType => RoomType.Shop;

    public void SpawnItems() {
        for (int i = 0; i < shopItems.Length; i++) {
            PickableInWorld.Place(shopItems[i], locations[i], true);
        }
    }
}
