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
    public StateCommunicator StateCommunicator { get; private set; }
    public DungeonRoom CurrentRoom { get; private set; }

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
    }

    private void Start()
    {
        PlayersDict.Instance.Register(this);
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

    [Command]
    public void CmdReviveTeammate(GameObject other)
    {
        if (other.TryGetComponent(out Player player) == false)
            return;

        Status.ServerReviving(player);
    }

    private void OnNameChanged(string oldName, string newName)
    {
        gameObject.name = newName;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out DungeonRoom dungeonRoom))
        {
            CurrentRoom = dungeonRoom;
            if (isLocalPlayer)
                dungeonRoom.OnLocalPlayerEntered();
            GameManager.OnPlayerChangedRoom(this);

            return;
        }

        if (isLocalPlayer && !Status.Dashing)
        {
            if (collision.TryGetComponent(out PickableInWorld pickable) && pickable.Pickable.InstantPickup)
                CmdPickup(pickable.gameObject);
            else if (collision.TryGetComponent(out Bullet bullet))
                CmdBulletHit(bullet.gameObject, bullet.fromWeapon);
            else if (collision.TryGetComponent(out Player player))
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
        if (LocalPlayer == this)
            LocalPlayer = null;
    }
}
