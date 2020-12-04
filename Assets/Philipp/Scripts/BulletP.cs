using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletP : MonoBehaviour
{
    [SerializeField]
    private float maxLifeTime = 5;

    private Rigidbody2D rb;
    public Vector2 vel;
    public bool owner;

    private float lifeTime = 0;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        lifeTime += Time.deltaTime;
        if (lifeTime > maxLifeTime)
            Destroy(gameObject);

        Move();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.transform.CompareTag("Enemy")) {
            if (owner) {
                Destroy(gameObject);
            }
        } else if (collision.transform.CompareTag("Player")) {
            if (!owner) {
                Destroy(gameObject);
            }
        } else {
            Destroy(gameObject);
        }
    }

    private void Move() {
        rb.velocity = new Vector3(vel.x, vel.y, 0);
    }
}
