using Mirror;
using Smooth;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquippedWeapon : NetworkBehaviour
{
    /// <summary>
    /// Checks if the weapon can be fired.
    /// </summary>
    public bool CanFire => weapon && HasBulletsLeft && !IsReloading && !IsFiring;
    /// <summary>
    /// Checks if the weapon should stop firing.
    /// </summary>
    public bool RequstStopFire => requstStopFire;
    /// <summary>
    /// Checks if the weapon is currently firing.
    /// </summary>
    public bool IsFiring => fireCoroutine != null && !fireCoroutine.IsFinshed;
    /// <summary>
    /// Checks if the weapon is currently reloading.
    /// </summary>
    public bool IsReloading => reloadCoroutine != null && !reloadCoroutine.IsFinshed;
    /// <summary>
    /// Checks if the weapon has any bullets left in the magazine.
    /// </summary>
    public bool HasBulletsLeft => remainingBullets > 0;

    public Weapon Weapon => weapon;
    public Health Health { get; private set; }
    public Vector2 Direction => direction;
    public int RemainingBullets => remainingBullets;
    public Vector2 BulletSpawnPosition => transform.position; // TODO: Change to acctual value

    [SerializeField] [SyncVar] private Weapon weapon;
    [SerializeField] int remainingBullets;
    [SerializeField] private bool requstStopFire;
    Vector2 direction;

    private ExtendedCoroutine fireCoroutine;
    private ExtendedCoroutine reloadCoroutine;

    private void Awake()
    {
        Health = GetComponent<Health>();
    }

    #region Events

    /// <summary>
    /// Called when a bullet was fired.
    /// </summary>
    public void OnFiredSingleShot()
    {
        --remainingBullets;
    }

    /// <summary>
    /// Called when the gun has stopped firing.
    /// </summary>
    public void OnStopFiring()
    {
        requstStopFire = false;
    }

    /// <summary>
    /// Called when the weapon has reloaded.
    /// </summary>
    public void OnReloaded()
    {
        remainingBullets = weapon.MagazineSize;
    }

    #endregion

    #region Input

    /// <summary>
    /// Swaps the current equipped weapon with a weapon on the level.
    /// </summary>
    /// <param name="newWeapon">The new weapon to be equipped.</param>
    [Server]
    public void Swap(Weapon newWeapon)
    {
        if (weapon != null)
            PickableInWorld.Place(weapon, transform.position);

        weapon = newWeapon;
        remainingBullets = weapon.MagazineSize;
        // Reset all timed values and stuff
    }

    /// <summary>
    /// Starts firing the gun.
    /// </summary>
    public bool StartFire()
    {
        if (!CanFire)
            return false;

        // Play shoot animation
        fireCoroutine = new ExtendedCoroutine(this, weapon.Shoot(this), OnStopFiring);
        fireCoroutine.Start();
        return true;
    }

    /// <summary>
    /// Sets the direction of the gun.
    /// </summary>
    public void SetDirection(Vector2 direction)
    {
        if (direction.x == 0 && direction.y == 0)
            return;
        direction.Normalize();
        this.direction = direction;
    }

    /// <summary>
    /// Stops firing the gun.
    /// </summary>
    public void StopFire()
    {
        requstStopFire = true;
    }

    /// <summary>
    /// Reloads the gun.
    /// </summary>
    public bool Reload()
    {
        if (weapon && !IsFiring && !IsReloading)
        {
            reloadCoroutine = new ExtendedCoroutine(this, StartReload(weapon.ReloadTime), OnReloaded);
            reloadCoroutine.Start();
            return true;
        }
        return false;
    }

    private IEnumerator StartReload(float reloadTime)
    {
        yield return new WaitForSeconds(reloadTime);
    }

    #endregion


    #region CallbackWhenBulletFired
    /// <summary>
    /// Requests a bullet to be spawned from when a player shoots.
    /// </summary>
    /// <param name="position">The position where the bullet should spawn.</param>
    /// <param name="direction">The direction where the bullet should head to.</param>
    [Command]
    public void CmdCreateBullet(Vector3 position, Vector2 direction)
    {
        if (gameObject.TryGetComponent(out Player player) == false)
            return;

        Bullet bullet = GetBullet(direction);
        bullet.owningPlayer = player.playerId;
        NetworkServer.Spawn(bullet.gameObject);
        // if this is set to the given position, then it might look weird for players who are lagging quite badly
        // if this is set to BulletSpawnPosition, then the bullets fly path is different to the player and server
        bullet.Ssm.setPosition(position, true);
    }

    /// <summary>
    /// Creates a new bullet. This should only be called from the BulletSpawnModel.
    /// </summary>
    public void SpawnNew()
    {
        Bullet bullet = GetBullet(Direction);
        if (isServer)
        {
            NetworkServer.Spawn(bullet.gameObject);
            bullet.GetComponent<SmoothSyncMirror>().teleport();
        }
        else
        {
            bullet.Ssm.enabled = false;
            bullet.owningBullet = true;
            CmdCreateBullet(BulletSpawnPosition, Direction);
        }
    }

    /// <summary>
    /// Gets a bullet from the bullet pool and adds all required info to the bullet.
    /// </summary>
    /// <param name="direction">The direction to where the bullet is fired off to.</param>
    /// <returns>The prepared bullet.</returns>
    private Bullet GetBullet(Vector2 direction)
    {
        Bullet bullet = Instantiate(PickableDict.Instance.BulletPrefab).GetComponent<Bullet>();

        bullet.gameObject.SetActive(true);

        bullet.ShooterHealth = Health;
        bullet.fromWeapon = weapon;
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bullet.OwnCollider, true);
        BulletInfo info = Weapon.BulletInfo;
        bullet.SpriteRenderer.sprite = info.Sprite;
        Vector2 velocity = direction * Weapon.Speed;
        bullet.Body.velocity = velocity;
        bullet.transform.position = BulletSpawnPosition;
        bullet.StartCoroutine(bullet.DeleteWhenOutOfRange(velocity, Weapon.Range));

        return bullet;
    }
    #endregion
}
