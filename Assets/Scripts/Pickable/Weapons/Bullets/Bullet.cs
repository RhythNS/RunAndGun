using Mirror;
using Smooth;
using System.Collections;
using UnityEngine;

public class Bullet : NetworkBehaviour, IPoolable
{
    private SpriteRenderer spriteRenderer;
    private SmoothSyncMirror ssm;
    private Rigidbody2D body;

    private Health shooterHealth;
    private Collider2D ownCollider;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ssm = GetComponent<SmoothSyncMirror>();
        body = GetComponent<Rigidbody2D>();
        ownCollider = GetComponent<Collider2D>();
    }

    public static void Set(EquippedWeapon equipped)
    {
        Bullet bullet = BulletPool.Instance.GetFromPool().GetComponent<Bullet>();
        bullet.gameObject.SetActive(true);


        bullet.shooterHealth = equipped.Health;
        Physics2D.IgnoreCollision(equipped.GetComponent<Collider2D>(), bullet.ownCollider, true);

        BulletInfo info = equipped.Weapon.BulletInfo;
        bullet.spriteRenderer.sprite = info.Sprite;
        Vector2 velocity = equipped.Direction * equipped.Weapon.Speed;
        bullet.body.velocity = velocity;
        bullet.StartCoroutine(bullet.DeleteWhenOutOfRange(velocity, equipped.Weapon.Range));
        NetworkServer.Spawn(bullet.gameObject);
        bullet.ssm.teleportAnyObjectFromServer(equipped.BulletSpawnPosition, bullet.transform.rotation, bullet.transform.localScale);
    }

    private IEnumerator DeleteWhenOutOfRange(Vector2 velocity, float range)
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
        if (shooterHealth)
        {
            Physics2D.IgnoreCollision(shooterHealth.GetComponent<Collider2D>(), ownCollider, false);
            StopAllCoroutines();
        }
        gameObject.SetActive(false);
    }

    public void Delete()
    {
        Destroy(gameObject);
    }
}
