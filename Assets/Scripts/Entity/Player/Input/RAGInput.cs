using UnityEngine;

public abstract class RAGInput : MonoBehaviour
{
    public abstract InputType InputType { get; }

    public float movementForce;
    public float MovementMultiplicator { get; private set; } = 1.0f;

    protected Rigidbody2D Body { get; private set; }
    protected Player Player { get; private set; }

    private PlayerCamera playerCamera;
    private bool useFocusPoint;

    private void Awake()
    {
        Body = GetComponent<Rigidbody2D>();
        Player = GetComponent<Player>();
        MovementMultiplicator = 1.0f;
    }

    private void Start()
    {
        movementForce = GetComponent<Stats>().GetMovementForce();
        playerCamera = Camera.main.GetComponent<PlayerCamera>();
        useFocusPoint = Config.Instance.useFocusPoint;
        OnStart(); // Call start on child classes.
    }

    public static RAGInput AttachInput(GameObject gameObject)
    {
        // Is there already a input instance on the player?
        if (gameObject.TryGetComponent(out RAGInput input))
            input.Remove();

        switch (Config.Instance.selectedInput)
        {
            case InputType.KeyMouse:
                return gameObject.AddComponent<KeyMouseInput>();
            /*
        case InputType.Keyboard:
            break;
        case InputType.Controller:
            break;
            */
            case InputType.Mobile:
                return gameObject.AddComponent<MobileInput>();
            default:
                Debug.LogError("InputType " + Config.Instance.selectedInput + " not found!");
                return null;
        }
    }

    private void Update()
    {
        if (useFocusPoint && HasFocusPoint)
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

        if (weapon.Weapon)
        {
            if (shouldFire)
            {
                if (weapon.CanFire)
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

    protected virtual Vector2 GetFocusPoint() { return new Vector2(0.0f, 0.0f); }

    /// <summary>
    /// Called after all internal values of RAGInput have been set. Should be treated as the
    /// Unity Start method.
    /// </summary>
    protected abstract void OnStart();

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

    protected abstract void Pickup();

    protected abstract bool GetReloadInput();

    protected abstract bool GetReviveInput();

    /// <summary>
    /// Removes the InputMethod from the player.
    /// </summary>
    public abstract void Remove();
}
