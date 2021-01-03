using UnityEngine;

public abstract class RAGInput : MonoBehaviour
{
    public abstract InputType InputType { get; }

    public float movementForce;

    protected Rigidbody2D Body { get; private set; }
    protected Player Player { get; private set; }

    private PlayerCamera playerCamera;

    private void Start()
    {
        movementForce = GetComponent<Stats>().GetMovementForce();
        Body = GetComponent<Rigidbody2D>();
        Player = GetComponent<Player>();
        playerCamera = Camera.main.GetComponent<PlayerCamera>();
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
        case InputType.Mobile:
            break;
            */
            default:
                Debug.LogError("InputType " + Config.Instance.selectedInput + " not found!");
                return null;
        }
    }

    private void Update()
    {
        if (HasFocusPoint)
            playerCamera.focusPoint = GetFocusPoint();

        // If the player is dashing dont listen to other input.
        if (Player.Status.IsDashing())
            return;

        // If we want to dash try to dash, if we are able dont listen to other input      .  
        if (GetDashInput() == true && Player.Status.TryDashing())
            return;

        // Handle weapon fire input.
        if (GetFireInput(out Vector2 fireDirection) && Player.EquippedWeapon.CanFire)
        {
            Player.EquippedWeapon.CmdFire(fireDirection);
        }
    }

    private void FixedUpdate()
    {
        if (Player.Status.IsDashing())
            return;
        Body.AddForce(GetMovementInput() * movementForce);
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

    /// <summary>
    /// Removes the InputMethod from the player.
    /// </summary>
    public abstract void Remove();
}
