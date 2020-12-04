using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    public void Set(GameObject shooter, Vector3 velocity, int damage, float aliveTime)
    {
        Destroy(gameObject, aliveTime);
        GetComponent<Rigidbody2D>().velocity = velocity;
        this.damage = damage;
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), shooter.GetComponent<Collider2D>());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Health health))
        {
            health.Damage(damage);
        }

        Destroy(gameObject);
    }

}
