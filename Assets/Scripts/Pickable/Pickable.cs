using UnityEngine;

public enum PickableType
{
    Consumable, Item, Weapon, StatusEffect
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

    public uint Costs => costs;
    [SerializeField] private uint costs = 0;
}
