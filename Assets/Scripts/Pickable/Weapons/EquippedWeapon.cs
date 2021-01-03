using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquippedWeapon : NetworkBehaviour
{
    // TODO: Implement
    public bool CanFire => true;

    public Weapon Weapon => weapon;
    [SyncVar] private Weapon weapon;

    [SyncVar] int remainingBullets;

    private ExtendedCoroutine shootCoroutine;
    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    [Server]
    public void OnFiredSingleShot()
    {
        --remainingBullets;
    }

    [Server]
    public void Swap(Weapon newWeapon)
    {
        if (weapon != null)
            PickableInWorld.Place(weapon, transform.position);

        weapon = newWeapon;
        // Reset all timed values and stuff
    }

    [Command]
    public void CmdFire(Vector2 fireDirection)
    {
        if (!weapon || !CanFire || remainingBullets <= 0 || !(shootCoroutine == null || shootCoroutine.IsFinshed))
            return;

        // Play shoot animation
        shootCoroutine = new ExtendedCoroutine(weapon.Shoot(health, this, transform.position, fireDirection));
        StartCoroutine(shootCoroutine);
    }

    [Command]
    public void Reload()
    {
        // Play reload animation
        remainingBullets = weapon.MagazineSize;
    }

}
