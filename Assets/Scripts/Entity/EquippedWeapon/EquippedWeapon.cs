using Mirror;
using Smooth;
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
    public Collider2D Collider2D { get; private set; }
    public Vector2 LocalDirection => localDirection;
    public int RemainingBullets => remainingBullets;
    public Vector2 BulletSpawnPosition => transform.position; // TODO: Change to acctual value
    public NetworkWeaponAnimator NetworkWeaponAnimator;

    [SerializeField] [SyncVar] private int bulletLayerSpawn;
    [SerializeField] [SyncVar(hook = nameof(OnWeaponChanged))] private Weapon weapon;
    [SerializeField] int remainingBullets;
    [SerializeField] private bool requstStopFire;
    private Vector2 localDirection;

    private ExtendedCoroutine fireCoroutine;
    private ExtendedCoroutine reloadCoroutine;

    private void Awake()
    {
        Health = GetComponent<Health>();
        Collider2D = GetComponent<Collider2D>();

        NetworkWeaponAnimator = GetComponent<NetworkWeaponAnimator>();
    }

    #region Events
    /// <summary>
    /// Called when a bullet was fired.
    /// </summary>
    public void OnFiredSingleShot()
    {
        --remainingBullets;
        NetworkWeaponAnimator.OnSingleShotFired();
    }

    /// <summary>
    /// Called when the gun has stopped firing.
    /// </summary>
    public void OnStopFiring()
    {
        requstStopFire = false;
        NetworkWeaponAnimator.OnStoppedFire();
    }

    /// <summary>
    /// Called when the weapon has reloaded.
    /// </summary>
    public void OnReloaded()
    {
        remainingBullets = weapon.MagazineSize;
        NetworkWeaponAnimator.OnStoppedReload();
    }

    /// <summary>
    /// Called when the weapon changed.
    /// </summary>
    /// <param name="_">The old weapon. This can be ignored.</param>
    /// <param name="newWeapon">The new changed weapon.</param>
    private void OnWeaponChanged(Weapon _, Weapon newWeapon)
    {
        NetworkWeaponAnimator.OnWeaponChanged(newWeapon);
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
        fireCoroutine = new ExtendedCoroutine(this, weapon.Shoot(this), OnStopFiring, true);

        NetworkWeaponAnimator.OnStartedFire();

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

        NetworkWeaponAnimator.SetDirection(direction);
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
            reloadCoroutine = ExtendedCoroutine.ActionAfterSeconds(this, weapon.ReloadTime, OnReloaded, true);
            NetworkWeaponAnimator.OnStartedReload();
            return true;
        }
        return false;
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
        bullet.IgnoreCollision(Collider2D);
        bullet.owningPlayer = player.playerId;
        // if this is set to the given position, then it might look weird for players who are lagging quite badly
        // if this is set to BulletSpawnPosition, then the bullets fly path is different to the player and server
        //bullet.Ssm.setPosition(position, true);
        bullet.transform.position = position;

        NetworkServer.Spawn(bullet.gameObject);
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
            //bullet.GetComponent<SmoothSyncMirror>().teleportAnyObjectFromServer(bullet.transform.position, bullet.transform.rotation, bullet.transform.localScale);
        }
        else
        {
            bullet.NetworkTransform.enabled = false;
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
        Bullet bullet = PoolDict.Instance.BulletPool.GetFromPool(BulletSpawnPosition, Quaternion.identity).GetComponent<Bullet>();

        bullet.gameObject.layer = bulletLayerSpawn;
        bullet.layer = (byte)bulletLayerSpawn;

        bullet.ShooterHealth = Health;
        bullet.fromWeapon = weapon;
        bullet.IgnoreCollision(Collider2D);
        BulletInfo info = Weapon.BulletInfo;
        bullet.SpriteRenderer.sprite = info.Sprite;
        Vector2 velocity = direction * Weapon.Speed;
        bullet.Body.velocity = velocity;
        // bullet.velocity = velocity;
        //bullet.transform.position = BulletSpawnPosition;
        bullet.StartCoroutine(bullet.DeleteWhenOutOfRange(velocity, Weapon.Range));

        return bullet;
    }
    #endregion
}
