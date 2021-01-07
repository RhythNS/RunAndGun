using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquippedWeapon : NetworkBehaviour
{
    // TODO: implement
    public bool CanFire => true;
    public bool Firing => firing;
    public bool RequstStopFire => requstStopFire;

    public Health Health { get; private set; }
    public Vector2 Direction => direction;
    public int RemainingBullets => remainingBullets;
    public Vector2 BulletSpawnPosition => transform.position; // TODO: Change to acctual value

    public Weapon Weapon => weapon;

    [SerializeField] [SyncVar] private Weapon weapon;
    [SerializeField] [SyncVar] int remainingBullets;
    [SerializeField] [SyncVar] private bool firing;
    [SerializeField] [SyncVar] private bool requstStopFire;
    [SyncVar] Vector2 direction;

    private ExtendedCoroutine shootCoroutine;

    private void Awake()
    {
        Health = GetComponent<Health>();
    }

    [Server]
    public void OnFiredSingleShot()
    {
        --remainingBullets;
    }

    public void OnStopFiring()
    {
        requstStopFire = false;
        firing = false;
    }

    [Server]
    public void Swap(Weapon newWeapon)
    {
        if (weapon != null)
            PickableInWorld.Place(weapon, transform.position);

        weapon = newWeapon;
        remainingBullets = weapon.MagazineSize;
        // Reset all timed values and stuff
    }

    [Command]
    public void CmdStartFire()
    {
        if (!weapon || !CanFire || remainingBullets <= 0 || !(shootCoroutine == null || shootCoroutine.IsFinshed))
            return;

        firing = true;
        // Play shoot animation
        shootCoroutine = new ExtendedCoroutine(this, weapon.Shoot(this));
        shootCoroutine.Start();
    }

    [Command]
    public void CmdSetDirection(Vector2 direction)
    {
        if (direction.x == 0 && direction.y == 0)
            return;
        direction.Normalize();
        this.direction = direction;
    }

    [Command]
    public void CmdStopFire()
    {
        requstStopFire = true;
    }

    [Command]
    public void CmdReload()
    {
        if (!Firing)
            remainingBullets = weapon.MagazineSize;
    }
}
