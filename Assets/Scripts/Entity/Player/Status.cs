using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : NetworkBehaviour
{
    private bool dashing = false;
    private float dashTimer = 0.0f;
    private float dashCooldown = 1.0f;

    [Server]
    public void SetDashCooldown(float cooldown)
    {
        dashCooldown = cooldown;
    }

    private void Update()
    {
        dashTimer -= Time.deltaTime;
    }

    public bool IsDashing()
    {
        return dashing;
    }

    public bool TryDashing()
    {
        if (dashing == true || dashTimer > 0.0f)
            return false;

        //TODO: Engage dashing

        return true;
    }
}
