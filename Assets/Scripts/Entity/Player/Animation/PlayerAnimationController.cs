using Mirror;
using UnityEngine;

/// <summary>
/// Helper class for animating the player.
/// </summary>
public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D body;
    private Status status;
    private EquippedWeapon weapon;
    private NetworkAnimator networkAnimator;

    private static readonly float DEADZONE = 0.1f;
    private bool prevDashing;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        status = GetComponent<Status>();
        weapon = GetComponent<EquippedWeapon>();
        networkAnimator = GetComponent<NetworkAnimator>();
        prevDashing = status.Dashing;
    }

    private void Start()
    {
        status.OnRevived += OnRevived;
        status.OnRevivingOtherPlayerFinished += OnRevivingOtherPlayerFinished;
        status.OnRevivingOtherPlayerStarted += OnRevivingOtherPlayerStarted;
    }

    /// <summary>
    /// Callback when the player died.
    /// </summary>
    public void OnDeath()
    {
        animator.SetBool("Dead", true);
        // Triggers must be set on the network animator
        networkAnimator.SetTrigger("DeadTrigger");
    }

    /// <summary>
    /// Callback when the player got revived.
    /// </summary>
    public void OnRevived()
    {
        animator.SetBool("Dead", false);
    }

    /// <summary>
    /// Callback when the player started reviving someone else.
    /// </summary>
    public void OnRevivingOtherPlayerStarted()
    {
        animator.SetBool("Reviving", true);
    }
    
    /// <summary>
    /// Callback when the player stopped reviving someone else.
    /// </summary>
    public void OnRevivingOtherPlayerFinished()
    {
        animator.SetBool("Reviving", false);
    }

    private void LateUpdate()
    {
        animator.SetFloat("Speed", body.velocity.magnitude);

        if (prevDashing == status.Dashing)
        {
            SetDirection(weapon.LocalDirection);
        }
        else
        {
            prevDashing = !prevDashing;

            SetDirection(body.velocity);
            animator.Update(0.01f);

            animator.SetBool("Dodging", prevDashing);
        }
    }

    private void SetDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x < -DEADZONE)
                animator.SetInteger("Direction", 3);
            else if (direction.x > DEADZONE)
                animator.SetInteger("Direction", 1);
        }
        else
        {
            if (direction.y < -DEADZONE)
                animator.SetInteger("Direction", 2);
            else if (direction.y > DEADZONE)
                animator.SetInteger("Direction", 0);
        }
    }

    private void OnDestroy()
    {
        if (status)
        {
            status.OnRevived -= OnRevived;
            status.OnRevivingOtherPlayerFinished -= OnRevivingOtherPlayerFinished;
            status.OnRevivingOtherPlayerStarted -= OnRevivingOtherPlayerStarted;
        }
    }
}
