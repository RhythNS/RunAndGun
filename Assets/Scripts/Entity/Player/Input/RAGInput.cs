using UnityEngine;

/// <summary>
/// Abstraction layer for the input. Inherit from this to implement inputs
/// for different controllers or plattforms.
/// </summary>
public abstract class RAGInput : MonoBehaviour
{
    public abstract InputType InputType { get; }

    /// <summary>
    /// The maximum force applied to the entity when moving.
    /// </summary>
    public float movementForce;
    /// <summary>
    /// Multiplicator that is added to the movment force. Can be used if items
    /// increase or decrease the movement speed.
    /// </summary>
    public float MovementMultiplicator { get; private set; } = 1.0f;

    protected Rigidbody2D Body { get; private set; }
    protected Player Player { get; private set; }

    private PlayerCamera playerCamera;

    /// <summary>
    /// Wheter the camera should take the mouse pointer into consideration.
    /// </summary>
    private bool useFocusPoint;

    private void Awake()
    {
        Body = GetComponent<Rigidbody2D>();
        Player = GetComponent<Player>();
        MovementMultiplicator = 1.0f;
    }

    public virtual void Start()
    {
        Stats stats = GetComponent<Stats>();
        stats.OnSpeedChanged += OnMovementStatChanged;
        OnMovementStatChanged(stats.Speed);

        playerCamera = Camera.main.GetComponent<PlayerCamera>();
        useFocusPoint = Config.Instance.useFocusPoint;
    }

    public virtual void OnDestroy()
    {
        Stats stats = GetComponent<Stats>();
        if (stats)
            stats.OnSpeedChanged -= OnMovementStatChanged;
    }

    public void OnMovementStatChanged(int movementStat)
    {
        movementForce = PlayerStatsDict.Instance.GetMovementForce(movementStat);
    }

    /// <summary>
    /// Attaches the specified input set in the config to the given object.
    /// </summary>
    /// <param name="gameObject">The gameobject to which the input is attached to.</param>
    /// <returns>A reference to the created input.</returns>
    public static RAGInput AttachInput(Player player)
    {
        // Is there already a input instance on the player?
        if (player.TryGetComponent(out RAGInput input))
            input.Remove();

        if (player.IsAI == true)
            return player.gameObject.AddComponent<AIInput>();

        switch (Config.Instance.selectedInput)
        {
            case InputType.KeyMouse:
                return player.gameObject.AddComponent<KeyMouseInput>();
            /*
        case InputType.Keyboard:
            break;
        case InputType.Controller:
            break;
            */
            case InputType.Mobile:
                return player.gameObject.AddComponent<MobileInput>();
            default:
                Debug.LogError("InputType " + Config.Instance.selectedInput + " not found!");
                return null;
        }
    }

    private void Update()
    {
        if (GetEmoteInput())
            UIManager.Instance.ToggleEmotePanel();

        //if (useFocusPoint && HasFocusPoint)
        playerCamera.focusPoint = GetFocusPoint();

        // If the player is dashing dont listen to other input.
        if (!Player.Status.CanInteract)
            return;

        // If we want to dash try to dash, if we are able dont listen to other input.  
        if (GetDashInput() == true && Player.Status.TryDashing())
            return;

        // Handle weapon fire input.
        EquippedWeapon weapon = Player.EquippedWeapon;

        bool shouldFire = GetFireInput(out Vector2 fireDirection);
        weapon.SetDirection(fireDirection);

        // Does the player have a weapon?
        if (weapon.Weapon)
        {
            if (shouldFire)
            {
                if (weapon.HasBulletsLeft == false)
                    Player.LocalSound.PlayWeaponCanNotShoot(weapon.Weapon.WeaponSoundModel.EmptyClipSound);
                else if (weapon.CanFire)
                    weapon.StartFire();
            }
            else
            {
                if (weapon.IsFiring && !weapon.RequestStopFire)
                {
                    weapon.StopFire();
                }
            }
        }

        // Let the implentation handle how they pick stuff up.
        Pickup();

        // Handle reload
        if (GetReloadInput())
            Player.EquippedWeapon.Reload();

        if (GetReviveInput())
            Player.Status.TryRevive();
    }

    private void FixedUpdate()
    {
        if (!Player.Status.CanInteract)
            return;
        Body.AddForce(GetMovementInput() * (movementForce * MovementMultiplicator));
    }

    /// <summary>
    /// Should only be called from Status.
    /// </summary>
    /// <param name="newValue">The new movement multiplicator.</param>
    public void SetMovementMultiplicator(float newValue)
    {
        MovementMultiplicator = newValue;
    }

    protected virtual bool HasFocusPoint => false;

    protected virtual Vector2 GetFocusPoint() { return transform.position; }

    /// <summary>
    /// Gets the direction to where the player wants to go.
    /// </summary>
    protected abstract Vector2 GetMovementInput();

    /// <summary>
    /// Returns wheter the player wants to dash or not.
    /// </summary>
    protected abstract bool GetDashInput();

    /// <summary>
    /// Gets the direction in which the player wants to fire to.
    /// </summary>
    /// <param name="fireDirection">The normalized direction.</param>
    /// <returns>Wheter the player wants to shoot or not.</returns>
    protected abstract bool GetFireInput(out Vector2 fireDirection);

    /// <summary>
    /// Calls when the player can pickup an item.
    /// </summary>
    protected abstract void Pickup();

    /// <summary>
    /// Returns wheter the player wants to reload or not.
    /// </summary>
    protected abstract bool GetReloadInput();

    /// <summary>
    /// Returns wheter the player wants to revive a nearby player.
    /// </summary>
    protected abstract bool GetReviveInput();

    /// <summary>
    /// Returns wheter the player wants to toggle the emote panel.
    /// </summary>
    protected virtual bool GetEmoteInput() { return false; }

    /// <summary>
    /// Removes the InputMethod from the player.
    /// </summary>
    public abstract void Remove();

}
