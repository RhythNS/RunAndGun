using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickableType
{
    Consumable, Item, Weapon
}

public abstract class Pickable : ScriptableObject
{
    public abstract PickableType PickableType { get; }

    public ushort Id => id;
    [SerializeField] private ushort id = 0;

    public Sprite Icon => icon;
    [SerializeField] private Sprite icon;

    public bool InstantPickup => instantPickup;
    [SerializeField] private bool instantPickup = false;
}
