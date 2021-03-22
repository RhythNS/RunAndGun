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

    public bool CanMove { get; private set; } = true;
    public RAGInput Input { get; private set; }
    public Stats Stats { get; private set; }
    public Status Status { get; private set; }
    public Health Health { get; private set; }
    public Inventory Inventory { get; private set; }
    public EquippedWeapon EquippedWeapon { get; private set; }
    public PlayerAnimationController PlayerAnimationController { get; private set; }
    public SmoothSyncMirror SmoothSync { get; private set; }
    public Collider2D Collider2D { get; private set; }
    public StateCommunicator StateCommunicator { get; private set; }
    public DungeonRoom CurrentRoom { get; private set; }
    public StatusEffectList StatusEffectList { get; private set; }
    public EntityMaterialManager EntityMaterialManager { get; private set; }

    private SpriteRenderer sr;

    private Vector3 lastPosition = Vector3.zero;
    public Vector3 LastPosition => lastPosition;

    private void Awake()
    {
        Stats = GetComponent<Stats>();
        Status = GetComponent<Status>();
        Health = GetComponent<Health>();
        Inventory = GetComponent<Inventory>();
        EquippedWeapon = GetComponent<EquippedWeapon>();
        SmoothSync = GetComponent<SmoothSyncMirror>();
        Collider2D = GetComponent<Collider2D>();
        StateCommunicator = GetComponent<StateCommunicator>();
        StatusEffectList = GetComponent<StatusEffectList>();
        EntityMaterialManager = GetComponent<EntityMaterialManager>();

        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        sr.sortingOrder = (int)transform.position.y * -2 + 1;
    }

    private void LateUpdate() {
        lastPosition = transform.position;
    }

    private void Start()
    {
        PlayersDict.Instance.Register(this);
        EntityMaterialManager.PlaySpawnEffect();
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
        UIManager.Instance.OnLocalPlayerStarted(this, Input.InputType);
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

        if (piw.IsBuyable) {
            if (Inventory.money >= pickable.Costs) {
                Inventory.money -= (int)pickable.Costs;
            } else {
                return;
            }
        }

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
            case PickableType.StatusEffect:
                Debug.LogError("Status Effect should not be on the ground... " + pickable.Id);
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
        if (gameObject)
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

    [TargetRpc]
    public void RpcChangeCanMove(bool canMove)
    {
        CanMove = canMove;
        // ui.CanMove(canMove);
        Input.enabled = canMove;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out DungeonRoom dungeonRoom))
        {
            CurrentRoom = dungeonRoom;
            if (isLocalPlayer)
                dungeonRoom.OnLocalPlayerEntered();
            GameManager.OnPlayerChangedRoom(this);

            return;
        }

        if (isLocalPlayer && !Status.Dashing)
        {
            if (other.TryGetComponent(out PickableInWorld pickable) && pickable.Pickable.InstantPickup)
                CmdPickup(pickable.gameObject);
            else if (other.TryGetComponent(out Bullet bullet))
                CmdBulletHit(bullet.gameObject, bullet.fromWeapon);
            else if (other.TryGetComponent(out Player player))
            {
                if (!player.Health.Alive)
                    Status.OnDownedPlayerInRangeToRevive(player);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out DungeonRoom dungeonRoom))
        {
            CurrentRoom = null;
            if (isLocalPlayer)
                dungeonRoom.OnLocalPlayerLeft();
            GameManager.OnPlayerChangedRoom(this);

            return;
        }
        else if (other.TryGetComponent(out Player player))
        {
            if (Status.downedPlayerAbleToRevive == player)
                Status.OnDownedPlayerNoLongerInRange();
        }
    }

    private void OnDestroy()
    {
        if (Camera.main && Camera.main.TryGetComponent(out PlayerCamera camera) && camera.ToFollow == transform)
            camera.ToFollow = null;
        if (PlayersDict.Instance)
            PlayersDict.Instance.DeRegister(this);
        if (UIManager.Instance)
            UIManager.Instance.OnLocalPlayerDeleted();
        if (LocalPlayer == this)
            LocalPlayer = null;
    }
}
