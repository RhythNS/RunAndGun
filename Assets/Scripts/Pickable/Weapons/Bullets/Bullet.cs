using Mirror;
using System.Collections;
using UnityEngine;

public class Bullet : NetworkBehaviour, IPoolable
{
    public SpriteRenderer SpriteRenderer { get; private set; }
    public Rigidbody2D Body { get; private set; }

    public Health ShooterHealth { get; set; }
    public Collider2D OwnCollider { get; private set; }
    public NetworkTransform NetworkTransform { get; private set; }

    //[SyncVar(hook = nameof(OnVelocityChanged))] public Vector2 velocity;
    [SyncVar] public Weapon fromWeapon;
    [SyncVar] public int owningPlayer = 0;
    [SyncVar] public byte layer;
    public bool owningBullet = false;
    private Collider2D ignoringCollider;

    public Vector2 velocity = Vector2.zero;

    private float aliveTime = 0f;

    private void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Body = GetComponent<Rigidbody2D>();
        OwnCollider = GetComponent<Collider2D>();
        NetworkTransform = GetComponent<NetworkTransform>();
    }

    public override void OnStartClient()
    {
        if (isServer)
            return;

        gameObject.layer = layer;
        if (Player.LocalPlayer?.playerId == owningPlayer)
            gameObject.SetActive(false);
        else
        {
            /*
            Debug.Log(velocity);
            Body.velocity = velocity;
             */
            /*
            Ssm.setPosition(transform.position, true);
            StateMirror stateMirror = new StateMirror();
            stateMirror.copyFromSmoothSync(Ssm);
            stateMirror.velocity = velocity;
            Debug.Log(Ssm.stateCount);
            Ssm.addState(stateMirror);
             */
        }
    }

    private void FixedUpdate() {
        aliveTime += Time.fixedDeltaTime;
        Vector2 vec = Quaternion.AngleAxis(fromWeapon.BulletPath.GetCurrentAngle(aliveTime), Vector3.forward) * velocity;
        Body.velocity = vec;
    }

    private void Update() {
        if (fromWeapon.UseLocalSpace) {
            Vector3 move = PlayersDict.Instance.Players[owningPlayer].transform.position - PlayersDict.Instance.Players[owningPlayer].LastPosition;
            this.transform.position += move;
        }
    }

    private void OnVelocityChanged(Vector2 _, Vector2 newVelocity)
    {
        Body.velocity = newVelocity;
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
            PoolDict.Instance.BulletPool.Free(gameObject);
        }
        else if (owningBullet)
        {
            PoolDict.Instance.BulletPool.Free(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out _))
            return;

        if (isServer || owningBullet)
            OnHit(collision.gameObject);
    }

    public void HitPlayer(Player player)
    {
        OnHit(player.gameObject);
    }

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

        Free();
    }

    public void IgnoreCollision(Collider2D other)
    {
        Physics2D.IgnoreCollision(ignoringCollider = other, OwnCollider, true);
    }

    public void Show()
    {
        aliveTime = 0f;
        owningBullet = false;
        gameObject.SetActive(true);
        NetworkTransform.enabled = true;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        if (ignoringCollider)
            Physics2D.IgnoreCollision(ignoringCollider, OwnCollider, false);
    }

    public void Delete()
    {
        Destroy(gameObject);
    }
}
