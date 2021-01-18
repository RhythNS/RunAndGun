using Mirror;
using System.Collections;
using UnityEngine;

public class Status : NetworkBehaviour
{
    public bool Dashing { get; private set; } = false;

    private Rigidbody2D body;

    private readonly float dashMuliplier = 2.0f;

    private float dashTimer = 0.0f;
    private float dashCooldown = 1.0f;
    private float dashDuration = 0.5f;

    private Vector2 dashVelocity;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    [Server]
    public void SetDashCooldown(float cooldown)
    {
        dashCooldown = cooldown;
    }

    private void Update()
    {
        if (Dashing)
        {
            body.velocity = dashVelocity;
            return;
        }

        dashTimer -= Time.deltaTime;
    }

    public bool TryDashing()
    {
        if (Dashing == true || dashTimer > 0.0f)
            return false;

        dashVelocity = body.velocity * dashMuliplier;

        StartCoroutine(Dash());

        return true;
    }

    private IEnumerator Dash()
    {
        Dashing = true;
        yield return new WaitForSeconds(dashDuration);
        Dashing = false;
        dashTimer = dashCooldown;
    }
}
