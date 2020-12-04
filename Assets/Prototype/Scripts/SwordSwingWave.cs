using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSwingWave : MonoBehaviour
{
    private int damage;
    private List<Health> alreadyAffected = new List<Health>();

    public void Set(GameObject shooter, float aliveTime, Vector2 localToMove, int damage)
    {
        this.damage = damage;
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), shooter.GetComponent<Collider2D>());
        transform.parent = shooter.transform;
        transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        StartCoroutine(Swing(aliveTime, localToMove));
    }

    private IEnumerator Swing(float aliveTime, Vector2 localToMove)
    {
        float timer = 0;
        bool cont = true;
        while(cont == true)
        {
            timer += Time.deltaTime;
            float perc = timer / aliveTime;
            if (perc >= 1.0f)
            {
                perc = 1.0f;
                cont = false;
            }
            transform.localPosition = Vector2.Lerp(new Vector2(0.0f, 0.0f), localToMove, perc);
            yield return null;
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Health health) == true && alreadyAffected.Contains(health) == false)
        {
            alreadyAffected.Add(health);
            health.Damage(damage);
        }
    }
}
