using UnityEngine;

public abstract class RAGInput : MonoBehaviour
{
    public abstract InputType InputType { get; }

    public float movementForce;

    protected Rigidbody2D body;
    protected Player Player { get; private set; }

    private void Start()
    {
        movementForce = GetComponent<Stats>().GetMovementForce();
        body = GetComponent<Rigidbody2D>();
        Player = GetComponent<Player>();
        OnStart(); // Call start on child classes.
    }

    public static void AttachInput(GameObject gameObject)
    {
        // Is there already a input instance on the player?
        if (gameObject.TryGetComponent(out RAGInput input))
            input.Remove();

        switch (Config.Instance.selectedInput)
        {
            case InputType.KeyMouse:
                gameObject.AddComponent<KeyMouseInput>();
                break;
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
                break;
        }
    }

    private void Update()
    {
        // If the player is dashing dont listen to other input.
        if (Player.Status.IsDashing())
            return;

        // If we want to dash try to dash, if we are able dont listen to other input      .  
        if (GetDashInput() == true && Player.Status.TryDashing())
            return;

        // Handle weapon fire input.
        if (GetFireInput(out Vector2 fireDirection))
        {
            // TODO: Uncomment when implemented
            //Player.EquippedWeapon.Fire(fireDirection);
        }
    }

    private void FixedUpdate()
    {
        if (Player.Status.IsDashing())
            return;
        body.AddForce(GetMovementInput() * movementForce);
    }

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
