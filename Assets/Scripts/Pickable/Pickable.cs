using UnityEngine;

/// <summary>
/// All subclasses of Pickable.
/// </summary>
public enum PickableType
{
    Consumable, Item, Weapon, StatusEffect, WorldEffect
}

/// <summary>
/// A scriptable object that can easily be synchronised over the network.
/// </summary>
public abstract class Pickable : ScriptableObject
{
    /// <summary>
    /// The type of this pickable.
    /// </summary>
    public abstract PickableType PickableType { get; }

    /// <summary>
    /// The network id.
    /// </summary>
    public ushort Id => id;
    [SerializeField] private ushort id = 0;

    /// <summary>
    /// Generic icon of the pickable.
    /// </summary>
    public Sprite Icon => icon;
    [SerializeField] private Sprite icon;

    /// <summary>
    /// Wheter it is instanly pickable.
    /// </summary>
    public bool InstantPickup => instantPickup;
    [SerializeField] private bool instantPickup = false;

    /// <summary>
    /// How much the pickable costs.
    /// </summary>
    public uint Costs => costs;
    [SerializeField] private uint costs = 0;
}
