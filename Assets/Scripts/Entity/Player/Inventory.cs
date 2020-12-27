using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : NetworkBehaviour
{
    public SyncList<Item> Items => items;
    private readonly SyncList<Item> items = new SyncList<Item>();

    [SyncVar] public int money;

    public void PickUp(Item item)
    {

    }

}
