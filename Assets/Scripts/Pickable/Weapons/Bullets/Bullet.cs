using Mirror;
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

    [SyncVar] public int owningPlayer = 0;

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

        if (Player.LocalPlayer?.playerId == owningPlayer)
            gameObject.SetActive(false);
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
            StopAllCoroutines();
        }
        gameObject.SetActive(false);
    }

    public void Delete()
    {
        Destroy(gameObject);
    }
}
