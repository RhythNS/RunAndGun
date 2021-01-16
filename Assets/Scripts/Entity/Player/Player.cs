using Mirror;
using Smooth;
using UnityEngine;

public class Player : Entity
{
    public CharacterType CharacterType => characterType;
    [SerializeField] private CharacterType characterType;

    public override EntityType EntityType => EntityType.Player;

    [SyncVar(hook = nameof(OnNameChanged))] public string userName;
    [SyncVar] public int playerId;

    public static Player LocalPlayer { get; private set; }

    public RAGInput Input { get; private set; }
    public Stats Stats { get; private set; }
    public Status Status { get; private set; }
    public Health Health { get; private set; }
    public Inventory Inventory { get; private set; }
    public EquippedWeapon EquippedWeapon { get; private set; }
    public PlayerAnimationController PlayerAnimationController { get; private set; }
    public SmoothSyncMirror SmoothSync { get; private set; }
    public Collider2D Collider2D { get; private set; }


    private void Awake()
    {
        Stats = GetComponent<Stats>();
        Status = GetComponent<Status>();
        Health = GetComponent<Health>();
        Inventory = GetComponent<Inventory>();
        EquippedWeapon = GetComponent<EquippedWeapon>();
        SmoothSync = GetComponent<SmoothSyncMirror>();
        Collider2D = GetComponent<Collider2D>();
    }

    public override void OnStartClient()
    {
        gameObject.name = userName;
    }

    public override void OnStartLocalPlayer()
    {
        LocalPlayer = this;
        Config.Instance.selectedPlayerType = characterType;
        Input = RAGInput.AttachInput(gameObject);
        Camera.main.GetComponent<PlayerCamera>().ToFollow = transform;
        PlayerAnimationController = gameObject.AddComponent<PlayerAnimationController>();
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
                EquippedWeapon.Swap((Weapon)pickable);
                break;
            default:
                Debug.LogError("Type " + pickable.PickableType + " not implemented!");
                break;
        }
        Destroy(pickup);
    }

    [Command]
    public void CmdBulletHit(GameObject gameObject, Weapon firedWeapon)
    {
        if (!gameObject)
        {
            Health health = GetComponent<Health>();
            for (int i = 0; i < firedWeapon.Effects.Length; i++)
            {
                firedWeapon.Effects[i].OnHit(firedWeapon, health);
            }
            return;
        }

        if (gameObject.TryGetComponent(out Bullet bullet) == false)
            return;

        bullet.HitPlayer(this);
    }

    private void OnNameChanged(string oldName, string newName)
    {
        gameObject.name = newName;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isLocalPlayer && !Status.Dashing)
        {
            if (collision.TryGetComponent(out PickableInWorld pickable) && pickable.Pickable.InstantPickup)
                CmdPickup(pickable.gameObject);
            else if (collision.TryGetComponent(out Bullet bullet))
                CmdBulletHit(bullet.gameObject, bullet.fromWeapon);
        }
    }

    private void OnDestroy()
    {
        if (Camera.main && Camera.main.TryGetComponent(out PlayerCamera camera))
            camera.ToFollow = null;
    }
}
