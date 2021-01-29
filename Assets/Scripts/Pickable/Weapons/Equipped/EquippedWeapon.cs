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
    public WeaponAnimator WeaponAnimator { get; private set; }
    public Health Health { get; private set; }
    public Collider2D Collider2D { get; private set; }
    public Vector2 LocalDirection => localDirection;
    public int RemainingBullets => remainingBullets;
    public Vector2 BulletSpawnPosition => transform.position; // TODO: Change to acctual value

    [SerializeField] [SyncVar] private int bulletLayerSpawn;
    [SerializeField] [SyncVar(hook = nameof(OnWeaponChanged))] private Weapon weapon;
    [SerializeField] int remainingBullets;
    [SerializeField] private bool requstStopFire;
    Vector2 localDirection;
    private bool serverAuthority;

    public Vector2 ServerDirection => serverDirection;
    [SyncVar(hook = nameof(OnDirectionChanged))] Vector2 serverDirection;

    private ExtendedCoroutine fireCoroutine;
    private ExtendedCoroutine reloadCoroutine;

    private void Awake()
    {
        Health = GetComponent<Health>();
        Collider2D = GetComponent<Collider2D>();
        serverAuthority = GetComponent<Entity>().EntityType == EntityType.Enemy;

        WeaponAnimator = GetComponentInChildren<WeaponAnimator>(); // TODO: <-- maybe change this
    }

    #region Events

    /// <summary>
    /// Called when a bullet was fired.
    /// </summary>
    public void OnFiredSingleShot()
    {
        --remainingBullets;
        WeaponAnimator.OnSingleShotFired();
        if (serverAuthority)
            RpcSingleShotFired();
        else
            CmdSingleShotFired();
    }

    /// <summary>
    /// Called when the gun has stopped firing.
    /// </summary>
    public void OnStopFiring()
    {
        requstStopFire = false;
        if (serverAuthority)
            RpcStoppedFire();
        else
            CmdStoppedFire();
        WeaponAnimator.OnStoppedFire();
    }

    /// <summary>
    /// Called when the weapon has reloaded.
    /// </summary>
    public void OnReloaded()
    {
        remainingBullets = weapon.MagazineSize;
        WeaponAnimator.OnStoppedReload();

        if (serverAuthority)
            RpcOnStoppedReload();
        else
            CmdOnStoppedReload();
    }

    #endregion

    #region CallbacksForAnimator
    [Command]
    public void CmdStartedFire()
    {
        RpcStartedFire();
    }

    [Command]
    public void CmdStoppedFire()
    {
        RpcStoppedFire();
    }

    [Command]
    public void CmdSingleShotFired()
    {
        RpcSingleShotFired();
    }

    [Command]
    public void CmdOnStartedReload()
    {
        RpcOnStartedReload();
    }

    [Command]
    public void CmdOnStoppedReload()
    {
        RpcOnStoppedReload();
    }

    [Command]
    public void CmdSetDirection(Vector2 direction)
    {
        serverDirection = direction;
    }

    [ClientRpc(excludeOwner = true)]
    public void RpcStartedFire()
    {
        WeaponAnimator.OnStartedFire();
    }

    [ClientRpc(excludeOwner = true)]
    public void RpcStoppedFire()
    {
        WeaponAnimator.OnStoppedFire();
    }

    [ClientRpc(excludeOwner = true)]
    public void RpcSingleShotFired()
    {
        WeaponAnimator.OnSingleShotFired();
    }

    [ClientRpc(excludeOwner = true)]
    public void RpcOnStartedReload()
    {
        WeaponAnimator.OnStartedReload();
    }

    [ClientRpc(excludeOwner = true)]
    public void RpcOnStoppedReload()
    {
        WeaponAnimator.OnStoppedReload();
    }

    public void OnDirectionChanged(Vector2 oldDir, Vector2 newDir)
    {
        if (!isLocalPlayer)
            WeaponAnimator.SetDirection(newDir);
    }

    private void OnWeaponChanged(Weapon oldWeapon, Weapon newWeapon)
    {
        WeaponAnimator = WeaponAnimator.Replace(WeaponAnimator, newWeapon);
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
        bulletLayerSpawn = LayerDict.Instance.GetBulletLayer(Health.EntityType, weapon.TargetMode);
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
        WeaponAnimator.OnStartedFire();

        if (serverAuthority)
            RpcStartedFire();
        else
            CmdStartedFire();
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
        localDirection = direction;

        if (serverAuthority)
            serverDirection = direction;
        else
            CmdSetDirection(direction);
        
        WeaponAnimator.SetDirection(direction);
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
            WeaponAnimator.OnStartedReload();

            if (serverAuthority)
                RpcOnStartedReload();
            else
                CmdOnStartedReload();
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
    /// <param name="layer">The layer on which the bullet spawns on.</param>
    [Command]
    private void CmdCreateBullet(Vector3 position, Vector2 direction)
    {
        if (gameObject.TryGetComponent(out Player player) == false)
            return;

        Bullet bullet = GetBullet(direction);
        bullet.gameObject.layer = LayerDict.Instance.GetBulletLayer(Health.EntityType, weapon.TargetMode);
        Physics2D.IgnoreCollision(Collider2D, bullet.OwnCollider, true);
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
        Bullet bullet = GetBullet(LocalDirection);
        if (isServer)
        {
            NetworkServer.Spawn(bullet.gameObject);
            bullet.GetComponent<SmoothSyncMirror>().teleport();
        }
        else
        {
            bullet.Ssm.enabled = false;
            bullet.owningBullet = true;
            CmdCreateBullet(BulletSpawnPosition, LocalDirection);
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
        bullet.gameObject.layer = bulletLayerSpawn;
        bullet.layer = (byte)bulletLayerSpawn;

        bullet.ShooterHealth = Health;
        bullet.fromWeapon = weapon;
        Physics2D.IgnoreCollision(Collider2D, bullet.OwnCollider, true);
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
