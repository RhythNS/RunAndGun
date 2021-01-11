﻿using Mirror;
using Smooth;
using System.Collections;
using UnityEngine;

public class Bullet : NetworkBehaviour, IPoolable
{
    public SpriteRenderer SpriteRenderer { get; private set; }
    public SmoothSyncMirror Ssm { get; private set; }
    public Rigidbody2D Body { get; private set; }

    public Health ShooterHealth { get; set; }
    public Collider2D OwnCollider { get; private set; }

    [SyncVar] public Weapon fromWeapon;
    [SyncVar] public int owningPlayer = 0;
    public bool owningBullet = false;

    private void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Ssm = GetComponent<SmoothSyncMirror>();
        Body = GetComponent<Rigidbody2D>();
        OwnCollider = GetComponent<Collider2D>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!isServer && Player.LocalPlayer?.playerId == owningPlayer)
            Free();
        else
            Ssm.enabled = true;
    }


    public IEnumerator DeleteWhenOutOfRange(Vector2 velocity, float range)
    {
        yield return new WaitForSeconds(range / velocity.magnitude);
        Free();
    }

    public void Free()
    {
        if (isServer)
        {
            NetworkServer.UnSpawn(gameObject);
        }
        BulletPool.Instance.Free(gameObject);
    }

    public void Hide()
    {
        if (ShooterHealth)
        {
            Physics2D.IgnoreCollision(ShooterHealth.GetComponent<Collider2D>(), OwnCollider, false);
            owningBullet = false;
            StopAllCoroutines();
        }
        gameObject.SetActive(false);
    }

    public void Delete()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out _))
            return;

        OnHit(collision.gameObject);
    }

    public void HitPlayer(Player player)
    {
        OnHit(player.gameObject);
    }

    // TODO: ADD FREE BULLET SOMEWHERE
    private void OnHit(GameObject collider)
    {
        if (collider.TryGetComponent(out Health health) && isServer)
        {
            for (int i = 0; i < fromWeapon.Effects.Length; i++)
            {
                fromWeapon.Effects[i].OnHit(fromWeapon, health);
            }
        }

        if (isServer || owningBullet)
        {
            for (int i = 0; i < fromWeapon.BulletInfo.BulletImpactEffects.Length; i++)
            {
                fromWeapon.BulletInfo.BulletImpactEffects[i].OnBulletImpacted(transform.position, health != null);
            }
        }
    }
}
