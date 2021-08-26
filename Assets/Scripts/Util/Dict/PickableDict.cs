using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dict for all types of all pickables.
/// </summary>
public class PickableDict : MonoBehaviour
{
    public static PickableDict Instance { get; private set; }

    [SerializeField] private Weapon[] weapons;
    [SerializeField] private Consumable[] consumables;
    [SerializeField] private Item[] items;
    [SerializeField] private StatusEffect[] statusEffects;
    [SerializeField] private WorldEffect[] worldEffects;

    public GameObject PickableInWorldPrefab => pickableInWorldPrefab;
    [SerializeField] private GameObject pickableInWorldPrefab;

    public Sprite MissingTexture => missingTexture;
    [SerializeField] private Sprite missingTexture;

    public GameObject BulletPrefab => bulletPrefab;
    [SerializeField] private GameObject bulletPrefab;

    private readonly Dictionary<int, Weapon> weaponDict = new Dictionary<int, Weapon>();
    private readonly Dictionary<int, Consumable> consumableDict = new Dictionary<int, Consumable>();
    private readonly Dictionary<int, Item> itemDict = new Dictionary<int, Item>();
    private readonly Dictionary<int, StatusEffect> statusEffectDict = new Dictionary<int, StatusEffect>();
    private readonly Dictionary<int, WorldEffect> worldEffectDict = new Dictionary<int, WorldEffect>();

    public int NumWeapons => weaponDict.Count;
    public int NumConsumables => consumableDict.Count;
    public int NumItems => itemDict.Count;
    public int NumStatusEffects => statusEffectDict.Count;
    public int NumWorldEffects => worldEffectDict.Count;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("PickableDict already in Scene! Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;

        for (int i = 0; i < weapons.Length; i++)
            weaponDict.Add(weapons[i].Id, weapons[i]);
        for (int i = 0; i < consumables.Length; i++)
            consumableDict.Add(consumables[i].Id, consumables[i]);
        for (int i = 0; i < items.Length; i++)
            itemDict.Add(items[i].Id, items[i]);
        for (int i = 0; i < statusEffects.Length; i++)
            statusEffectDict.Add(statusEffects[i].Id, statusEffects[i]);
        for (int i = 0; i < worldEffects.Length; i++)
            worldEffectDict.Add(worldEffects[i].Id, worldEffects[i]);
    }

    /// <summary>
    /// Get a pickable based on its type and network id.
    /// </summary>
    /// <param name="type">The type of the pickable.</param>
    /// <param name="id">The network id of the pickable.</param>
    /// <returns>The pickable.</returns>
    public Pickable Get(PickableType type, int id)
    {
        if (id == 0)
            return null;
        switch (type)
        {
            case PickableType.Consumable:
                return consumableDict[id];
            case PickableType.Item:
                return itemDict[id];
            case PickableType.Weapon:
                return weaponDict[id];
            case PickableType.StatusEffect:
                return statusEffectDict[id];
            case PickableType.WorldEffect:
                return worldEffectDict[id];
        }
        throw new Exception("Could not find " + type);
    }

    /// <summary>
    /// Gets a consumable based on its network id.
    /// </summary>
    /// <param name="id">The network id of the consumable.</param>
    /// <returns>The gotten consumable.</returns>
    public Consumable GetConsumbale(int id) => id == 0 ? null : consumableDict[id];

    /// Gets a weapon based on its network id.
    /// </summary>
    /// <param name="id">The network id of the weapon.</param>
    /// <returns>The gotten weapon.</returns>
    public Weapon GetWeapon(int id) => id == 0 ? null : weaponDict[id];

    /// Gets a status effect based on its network id.
    /// </summary>
    /// <param name="id">The network id of the status effect.</param>
    /// <returns>The gotten status effect.</returns>
    public StatusEffect GetStatusEffect(ushort id) => id == 0 ? null :  Instantiate(statusEffectDict[id]);

    /// Gets an item based on its network id.
    /// </summary>
    /// <param name="id">The network id of the item.</param>
    /// <returns>The gotten item.</returns>
    public Item GetItem(int id) => id == 0 ? null : Instantiate(itemDict[id]);

    /// Gets a world effect based on its network id.
    /// </summary>
    /// <param name="id">The network id of the world effect.</param>
    /// <returns>The gotten world effect.</returns>
    public WorldEffect GetWorldEffect(ushort id) => id == 0 ? null : Instantiate(worldEffectDict[id]);

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
