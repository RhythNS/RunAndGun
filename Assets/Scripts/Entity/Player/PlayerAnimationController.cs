using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D body;
    private Status status;
    private EquippedWeapon weapon;

    private static readonly float DEADZONE = 0.1f;
    private bool prevDashing;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        status = GetComponent<Status>();
        weapon = GetComponent<EquippedWeapon>();
        prevDashing = status.Dashing;
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
}
