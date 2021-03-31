using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : NetworkBehaviour
{
    private Player player;

    public SyncList<Item> Items => items;
    private readonly SyncList<Item> items = new SyncList<Item>();

    [SyncVar] public int money;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    [Server]
    public void PickUp(Item item)
    {
        item.OnPickUp(player);
        items.Add(item);
        player.Stats.OnItemsChanged(items);
    }

    [Command]
    public void CmdDropItem(Item item, bool placeInWorld)
    {
        if (items.Contains(item) == false)
            return;
        item.OnDrop(player);
        items.Remove(item);
        player.Stats.OnItemsChanged(items);

        if (placeInWorld)
            PickableInWorld.Place(item, transform.position);
    }

    [Command]
    public void CmdBuyItem(int price)
    {
        if (money >= price)
            money -= price;
    }
}
