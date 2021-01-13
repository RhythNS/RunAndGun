using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D body;
    private Status status;
    private EquippedWeapon weapon;

    private static readonly float DEADZONE = 0.1f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        status = GetComponent<Status>();
        weapon = GetComponent<EquippedWeapon>();
    }

    private void LateUpdate()
    {
        Vector2 direction = weapon.Direction;

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
        animator.SetFloat("Speed", body.velocity.magnitude);

        animator.SetBool("Dodging", status.IsDashing());
    }
}
