using System.Collections;
using UnityEngine;

public class Missle : MonoBehaviour
{
    public float range;
    public int damage;

    public void Set(GameObject shooter, Vector3 velocity, int damage, float range, float aliveTime)
    {
        StartCoroutine(DestroySelf(aliveTime));
        GetComponent<Rigidbody2D>().velocity = velocity;
        this.damage = damage;
        this.range = range;
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), shooter.GetComponent<Collider2D>());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Explode();
    }

    private void Explode()
    {
        Instantiate(WeaponDict.Instance.explosionPrefab, transform.position, Quaternion.identity);

        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, range);

        for (int i = 0; i < colls.Length; i++)
        {
            if (colls[i].gameObject.TryGetComponent(out Health health))
            {
                health.Damage(damage);
            }
        }

        Destroy(gameObject);
    }

    private IEnumerator DestroySelf(float aliveTime)
    {
        yield return new WaitForSeconds(aliveTime);
        Explode();
    }
}
