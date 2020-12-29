using Mirror;
using Smooth;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public CharacterType CharacterType => characterType;
    [SerializeField] private CharacterType characterType;

    public RAGInput Input { get; private set; }
    public Stats Stats { get; private set; }
    public Status Status { get; private set; }
    public Health Health { get; private set; }
    public Inventory Inventory { get; private set; }
    //public EquippedWeapon EquippedWeapon { get; private set; }
    public SmoothSyncMirror SmoothSync { get; private set; }

    private void Awake()
    {
        Stats = GetComponent<Stats>();
        Status = GetComponent<Status>();
        Health = GetComponent<Health>();
        Inventory = GetComponent<Inventory>();
        SmoothSync = GetComponent<SmoothSyncMirror>();
    }

    public override void OnStartLocalPlayer()
    {
        Config.Instance.selectedPlayerType = characterType;
        Input = RAGInput.AttachInput(gameObject);
    }

    /// <summary>
    /// Picks up a PickableInWorld.
    /// </summary>
    /// <param name="pickup">The gameobject to be picked up. This must have the PickableInWorld component on it!</param>
    [Command]
    public void CmdPickup(GameObject pickup)
    {
        if (!pickup.TryGetComponent(out PickableInWorld piw))
            return;
        Pickable pickable = piw.Pickable;
        switch (pickable.PickableType)
        {
            case PickableType.Consumable:
                ((Consumable)pickable).Affect(this);
                break;
            case PickableType.Item:
                Inventory.PickUp((Item)pickable);
                break;
            case PickableType.Weapon:
                // EquippedWeapon.Swap((Weapon)pickable);
                break;
            default:
                Debug.LogError("Type " + pickable.PickableType + " not implemented!");
                break;
        }
    }

}
