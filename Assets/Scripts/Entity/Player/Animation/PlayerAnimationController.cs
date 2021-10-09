using Mirror;
using UnityEngine;

/// <summary>
/// Helper class for animating the player.
/// </summary>
public class PlayerAnimationController : EntityAnimationController
{
    private Status status;

    private bool prevDashing;

    protected override void Awake()
    {
        base.Awake();
        status = GetComponent<Status>();
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
        Animator.SetBool("Dead", true);
        // Triggers must be set on the network animator
        NetworkAnimator.SetTrigger("DeadTrigger");
    }

    /// <summary>
    /// Callback when the player got revived.
    /// </summary>
    public void OnRevived()
    {
        Animator.SetBool("Dead", false);
    }

    /// <summary>
    /// Callback when the player started reviving someone else.
    /// </summary>
    public void OnRevivingOtherPlayerStarted()
    {
        Animator.SetBool("Reviving", true);
    }

    /// <summary>
    /// Callback when the player stopped reviving someone else.
    /// </summary>
    public void OnRevivingOtherPlayerFinished()
    {
        Animator.SetBool("Reviving", false);
    }

    private void LateUpdate()
    {
        Animator.SetFloat("Speed", Body.velocity.magnitude);

        if (prevDashing == status.Dashing)
        {
            SetDirection(Weapon.LocalDirection);
        }
        else
        {
            prevDashing = !prevDashing;

            SetDirection(Body.velocity);
            Animator.Update(0.01f);

            Animator.SetBool("Dodging", prevDashing);
        }
    }

    protected override void InnerOnDirectionChanged(Direction direction)
    {
        Animator.SetInteger("Direction", (int)direction);
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
