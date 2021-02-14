using System;
using System.Collections.Generic;
using UnityEngine;

public class PickableDict : MonoBehaviour
{
    public static PickableDict Instance { get; private set; }

    [SerializeField] private Weapon[] weapons;
    [SerializeField] private Consumable[] consumables;
    [SerializeField] private Item[] items;
    [SerializeField] private StatusEffect[] statusEffects;

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
    }

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
                return statusEffects[id];
        }
        throw new Exception("Could not find " + type);
    }

    public Consumable GetConsumbale(int id) => id == 0 ? null : consumableDict[id];

    public Weapon GetWeapon(int id) => id == 0 ? null : weaponDict[id];

    public StatusEffect GetStatusEffect(ushort id) => id == 0 ? null :  Instantiate(statusEffectDict[id]);

    public Item GetItem(int id) => id == 0 ? null : Instantiate(itemDict[id]);

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
