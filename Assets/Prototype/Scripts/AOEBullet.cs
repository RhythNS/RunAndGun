using System.Collections;
using UnityEngine;

public class AOEBullet : MonoBehaviour
{
    private int remainingLayers;
    private float aliveTime;
    private Vector3 velocity;
    float velocityAngleChange;
    int bulletsFromStray;
    private int damage;
    private float speed;
    Collider2D shooter;

    public void Set(Collider2D shooter, bool splitNow, float velocityAngleChange, int bulletsFromStray, float speed, int remainingLayers, float aliveTime, Vector3 velocity, int damage)
    {
        this.velocityAngleChange = velocityAngleChange;
        this.bulletsFromStray = bulletsFromStray;
        this.remainingLayers = remainingLayers;
        this.aliveTime = aliveTime;
        this.speed = speed;
        this.velocity = velocity.normalized * speed;
        this.damage = damage;
        this.shooter = shooter;
        if (shooter != null)
            Physics2D.IgnoreCollision(shooter, GetComponent<Collider2D>());
        if (splitNow == true)
            Split();
        else
            StartCoroutine(Fly());
    }

    private void Split()
    {
        if (remainingLayers == 0)
        {
            Destroy(gameObject);
            return;
        }
        remainingLayers--;
        
        for (float i = -velocityAngleChange * bulletsFromStray / 2; i < velocityAngleChange * bulletsFromStray / 2; i += velocityAngleChange)
        {
            Vector3 velocity = Rotate(this.velocity, i);
            Instantiate(WeaponDict.Instance.aoeBullet, transform.position, Quaternion.identity).Set(shooter, false, velocityAngleChange, bulletsFromStray, speed, remainingLayers, aliveTime, velocity, damage);
        }

        Destroy(gameObject);
        return;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Health health))
        {
            health.Damage(damage);
        }

        Destroy(gameObject);
    }


    private IEnumerator Fly()
    {
        GetComponent<Rigidbody2D>().velocity = velocity;
        yield return new WaitForSeconds(aliveTime);
        Split();
    }

    public static Vector2 Rotate(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
}
