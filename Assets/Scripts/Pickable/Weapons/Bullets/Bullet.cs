using FMOD.Studio;
using FMODUnity;
using Mirror;
using System.Collections;
using UnityEngine;

/// <summary>
/// The projectile shoot from a weapon.
/// </summary>
public class Bullet : NetworkBehaviour, IPoolable
{
    public SpriteRenderer SpriteRenderer { get; private set; }
    public Rigidbody2D Body { get; private set; }

    public Health ShooterHealth { get; set; }
    public Collider2D OwnCollider { get; private set; }
    public NetworkTransform NetworkTransform { get; private set; }

    //[SyncVar(hook = nameof(OnVelocityChanged))] public Vector2 velocity;
    [SyncVar] public Weapon fromWeapon;
    [SyncVar] public int owningPlayer = -1;
    [SyncVar] public byte layer;
    [SyncVar] public GameObject shooterObject;

    public bool owningBullet = false;
    private Collider2D ignoringCollider;

    public Vector2 velocity = Vector2.zero;

    private float aliveTime = 0f;

    private EventInstance inAirLoop;

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
        ShooterHealth = shooterObject.GetComponent<Health>();
        SpriteRenderer.sprite = fromWeapon.BulletInfo.Sprite;

        if (fromWeapon != null && string.IsNullOrEmpty(fromWeapon.WeaponSoundModel.InAirLoop) == false)
        {
            inAirLoop = RuntimeManager.CreateInstance(fromWeapon.WeaponSoundModel.InAirLoop);
            inAirLoop.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
            inAirLoop.start();
            FMODUtil.PlayOnPosition(fromWeapon.WeaponSoundModel.ImpactSound, transform.position);
        }

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

    /*
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
     */

    /// <summary>
    /// Deletes the bullet after the bullet reached its max range.
    /// </summary>
    /// <param name="velocity">The velocity of the bullet.</param>
    /// <param name="range">How many units the bullets should travel for.</param>
    public IEnumerator DeleteWhenOutOfRange(Vector2 velocity, float range)
    {
        yield return new WaitForSeconds(range / velocity.magnitude);

        DoImpactEffects(false);

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

    /// <summary>
    /// Called when the bullet hit a local player.
    /// </summary>
    /// <param name="player">The player hit.</param>
    public void HitPlayer(Player player)
    {
        OnHit(player.gameObject);
    }

    /// <summary>
    /// Called when the bullet hit another collider.
    /// </summary>
    /// <param name="collider">The collider hit.</param>
    private void OnHit(GameObject collider)
    {
        if (collider.TryGetComponent(out Health health) && isServer)
        {
            if (ShooterHealth != null && ShooterHealth.TryGetComponent(out Player player) == true)
                StatTracker.Instance.GetStat<ShotsHitStat>(player).Add(1);

            for (int i = 0; i < fromWeapon.Effects.Length; i++)
            {
                fromWeapon.Effects[i].OnHit(fromWeapon, ShooterHealth, health);
            }
        }

        DoImpactEffects(health != null);

        Free();
    }

    /// <summary>
    /// Spawns impact effects.
    /// </summary>
    /// <param name="hitHealth">Wheter another entity was hit.</param>
    private void DoImpactEffects(bool hitHealth)
    {
        if (isServer || owningBullet)
        {
            for (int i = 0; i < fromWeapon.BulletInfo.BulletImpactEffects.Length; i++)
            {
                fromWeapon.BulletInfo.BulletImpactEffects[i].OnBulletImpacted(transform.position, ShooterHealth, hitHealth);
            }
        }
    }

    /// <summary>
    /// Ignores collision with another specified collider.
    /// </summary>
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
        if (fromWeapon != null && string.IsNullOrEmpty(fromWeapon.WeaponSoundModel.ImpactSound) == false)
            FMODUtil.PlayOnPosition(fromWeapon.WeaponSoundModel.ImpactSound, transform.position);

        if (inAirLoop.isValid() == true)
        {
            inAirLoop.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            inAirLoop.release();
        }

        owningPlayer = -1;
        gameObject.SetActive(false);
        if (ignoringCollider)
            Physics2D.IgnoreCollision(ignoringCollider, OwnCollider, false);
        ignoringCollider = null;
    }

    public void Delete()
    {
        Destroy(gameObject);
    }
}
