﻿using FMODUnity;
using Mirror;
using Smooth;
using UnityEngine;

/// <summary>
/// The entity that is controlled by each player.
/// </summary>
public class Player : Entity
{
    public CharacterType CharacterType => characterType;
    [SerializeField] private CharacterType characterType;

    public override EntityType EntityType => EntityType.Player;

    /// <summary>
    /// ID for network connection
    /// </summary>
    [SyncVar] public int playerId;
    [SyncVar] public int playerIndex;
    public string uniqueIdentifier;

    [SerializeField] [EventRef] private string itemPickUpSound;

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
    public EmoteCommunicator EmoteCommunicator { get; private set; }
    public DungeonRoom CurrentRoom { get; private set; }
    public StatusEffectList StatusEffectList { get; private set; }
    public EntityMaterialManager EntityMaterialManager { get; private set; }
    public LocalSound LocalSound { get; private set; }

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
        EmoteCommunicator = GetComponent<EmoteCommunicator>();
        StatusEffectList = GetComponent<StatusEffectList>();
        EntityMaterialManager = GetComponent<EntityMaterialManager>();
        LocalSound = GetComponent<LocalSound>();
    }

    private void Update()
    {
        PositionConverter.AdjustZ(transform);
    }

    private void LateUpdate()
    {
        lastPosition = transform.position;
    }

    private void Start()
    {
        PlayersDict.Instance.Register(this);
        EntityMaterialManager.PlaySpawnEffect();
    }

    public override void OnStartLocalPlayer()
    {
        LocalPlayer = this;
        Config.Instance.SelectedPlayerType = characterType;
        Input = RAGInput.AttachInput(gameObject);
        UIManager.Instance.OnLocalPlayerStarted(this, Input.InputType);
        MusicManager.Instance.RegisterPlayer(this);
        Camera.main.GetComponent<PlayerCamera>().ToFollow = transform;
        PlayerAnimationController = gameObject.AddComponent<PlayerAnimationController>();
        gameObject.AddComponent<StudioListener>();
    }

    /// <summary>
    /// Picks up a PickableInWorld.
    /// </summary>
    /// <param name="pickup">The gameobject to be picked up. This must have the PickableInWorld component on it!</param>
    [Command]
    public void CmdPickup(GameObject pickup)
    {
        if (!pickup.TryGetComponent(out PickableInWorld piw) || piw.Available == false)
            return;

        piw.PickedUp();
        Pickable pickable = piw.Pickable;

        if (piw.IsBuyable)
        {
            if (Inventory.money < pickable.Costs)
                return;

            Inventory.money -= (int)pickable.Costs;
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

        RpcItemPickedUp(pickable);
    }

    /// <summary>
    /// Called when the player picked up an item.
    /// </summary>
    /// <param name="pickable">The picked up item.</param>
    [TargetRpc]
    private void RpcItemPickedUp(Pickable pickable)
    {
        switch (pickable.PickableType)
        {
            case PickableType.Consumable:
            case PickableType.Item:
                FMODUtil.PlayOnTransform(itemPickUpSound, transform);
                break;
            case PickableType.Weapon:
                FMODUtil.PlayOnTransform(((Weapon)pickable).WeaponSoundModel.EquipSound, transform);
                break;
        }
    }

    /// <summary>
    /// Notifies the server that the player has been hit by a bullet.
    /// </summary>
    /// <param name="gameObject">The bullet that hit the player.</param>
    /// <param name="affecterObj">The entity that shoot the bullet.</param>
    /// <param name="firedWeapon">The weapon that fired the bullet.</param>
    [Command]
    public void CmdBulletHit(GameObject gameObject, GameObject affecterObj, Weapon firedWeapon)
    {
        // Check if the Player was hit by a bullet that did not already hit something else.
        if (gameObject && gameObject.TryGetComponent(out Bullet bullet) == true && gameObject.activeInHierarchy)
        {
            bullet.HitPlayer(this);
            return;
        }

        Health affecter = null;
        if (affecterObj != null)
            affecter = affecterObj.GetComponent<Health>();

        // Bullet already hit something else. In this case we just decrease the health and dont
        // do anything with the bullet.
        for (int i = 0; i < firedWeapon.Effects.Length; i++)
        {
            firedWeapon.Effects[i].OnHit(firedWeapon, affecter, Health);
        }
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
            {
                CmdBulletHit(bullet.gameObject, bullet.shooterObject, bullet.fromWeapon);
            }
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

        if (LocalPlayer != this)
            return;

        LocalPlayer = null;
        if (UIManager.Instance)
            UIManager.Instance.OnLocalPlayerDeleted();
        if (MusicManager.Instance)
            MusicManager.Instance.DeRegisterPlayer();
    }
}
