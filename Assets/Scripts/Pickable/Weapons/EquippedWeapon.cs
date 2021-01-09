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
    [SerializeField] int remainingBullets;
    [SerializeField] private bool firing;
    [SerializeField] private bool requstStopFire;
    Vector2 direction;

    private ExtendedCoroutine shootCoroutine;

    private void Awake()
    {
        Health = GetComponent<Health>();
    }

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

    public void StartFire()
    {
        if (!weapon || !CanFire || remainingBullets <= 0 || !(shootCoroutine == null || shootCoroutine.IsFinshed))
            return;

        firing = true;
        // Play shoot animation
        shootCoroutine = new ExtendedCoroutine(this, weapon.Shoot(this));
        shootCoroutine.Start();
    }

    public void SetDirection(Vector2 direction)
    {
        if (direction.x == 0 && direction.y == 0)
            return;
        direction.Normalize();
        this.direction = direction;
    }

    public void StopFire()
    {
        requstStopFire = true;
    }

    public void CmdReload()
    {
        if (!Firing)
            remainingBullets = weapon.MagazineSize;
    }




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

    public void SpawnNew()
    {
        Bullet bullet = GetBullet(Direction);
        if (bullet.isServer)
        {
            NetworkServer.Spawn(bullet.gameObject);
            bullet.Ssm.setPosition(BulletSpawnPosition, true);
        }
        else
        {
            bullet.Ssm.enabled = false;
            bullet.transform.position = BulletSpawnPosition;
            CmdCreateBullet(BulletSpawnPosition, Direction);
        }
    }

    private Bullet GetBullet(Vector2 direction)
    {
        Bullet bullet = BulletPool.Instance.GetFromPool().GetComponent<Bullet>();

        bullet.gameObject.SetActive(true);

        bullet.ShooterHealth = Health;
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bullet.OwnCollider, true);

        BulletInfo info = Weapon.BulletInfo;
        bullet.SpriteRenderer.sprite = info.Sprite;
        Vector2 velocity = direction * Weapon.Speed;
        bullet.Body.velocity = velocity;
        bullet.StartCoroutine(bullet.DeleteWhenOutOfRange(velocity, Weapon.Range));

        return bullet;
    }
}
