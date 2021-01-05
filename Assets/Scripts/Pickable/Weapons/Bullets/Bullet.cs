using Mirror;
using Smooth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : NetworkBehaviour, IPoolable
{
    private SpriteRenderer spriteRenderer;
    private SmoothSyncMirror ssm;
    private Rigidbody2D body;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ssm = GetComponent<SmoothSyncMirror>();
        body = GetComponent<Rigidbody2D>();
    }

    public static void Set(Health shooter, EquippedWeapon equipped, Vector3 position, Vector2 direction)
    {
        Bullet bullet = BulletPool.Instance.Get().GetComponent<Bullet>();
        bullet.gameObject.SetActive(true);

        BulletInfo info = equipped.Weapon.BulletInfo;
        bullet.spriteRenderer.sprite = info.Sprite;
        bullet.ssm.teleportAnyObjectFromServer(position, bullet.transform.rotation, bullet.transform.localScale);
        Vector2 velocity = direction * equipped.Weapon.Speed;
        bullet.body.velocity = velocity;
        bullet.StartCoroutine(bullet.DeleteWhenOutOfRange(velocity, equipped.Weapon.Range));
    }

    private IEnumerator DeleteWhenOutOfRange(Vector2 velocity, float range)
    {
        yield return new WaitForSeconds(range / velocity.magnitude);
        BulletPool.Instance.Free(gameObject);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Delete()
    {
        Destroy(gameObject);
    }
}
