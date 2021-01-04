using Mirror;
using Smooth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : NetworkBehaviour, IPoolable
{
    private SpriteRenderer spriteRenderer;
    private SmoothSyncMirror ssm;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ssm = GetComponent<SmoothSyncMirror>();
    }

    public static void Set(Health shooter, Sprite sprite, Vector3 position, Vector2 direction, float speed, bool useLocalSpace)
    {
        Bullet bullet = BulletPool.Instance.Get();
        bullet.spriteRenderer.sprite = sprite;
        bullet.ssm.teleportAnyObjectFromServer(position, bullet.transform.rotation, bullet.transform.localScale);
        // TODO: implement


        bullet.gameObject.SetActive(true);
    }

    public void Reset()
    {
        gameObject.SetActive(false);
    }

    public void Delete()
    {
        Destroy(gameObject);
    }
}
