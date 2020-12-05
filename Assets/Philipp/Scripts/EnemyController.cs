using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private LayerMask layerMask;

    private Rigidbody2D rb;

    float update = 0;
    float speed = 200;

    float fixedCooldown = 2;
    float cooldown = 0;
    bool canShoot = false;

    public int health = 100;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        if (!canShoot) {
            cooldown += Time.deltaTime;
            if (cooldown > fixedCooldown) {
                cooldown = 0;
                canShoot = true;
            }
        }

        if (canShoot)
            TryShoot();
    }

    private void FixedUpdate() {
        update += Time.deltaTime;
        if (update < 0.2f)
            return;

        update = 0;

        Move();
    }

    private void Move() {
        Vector2 directionToTarget = transform.position - target.position;
        float distance = directionToTarget.magnitude;
        directionToTarget.Normalize();

        if (distance < 3.5f) {
            // move away from player
            rb.velocity = new Vector3(directionToTarget.x, directionToTarget.y, 0) * speed * Time.fixedDeltaTime;
        } else if (distance > 5) {
            // move towards player
            rb.velocity = - new Vector3(directionToTarget.x, directionToTarget.y, 0) * speed * Time.fixedDeltaTime;
        } else {
            rb.velocity = Vector3.zero;
        }
    }

    private void TryShoot() {
        Vector2 origin = new Vector2(transform.position.x, transform.position.y);
        Vector2 direction = new Vector2(target.position.x, target.position.y) - new Vector2(transform.position.x, transform.position.y);
        RaycastHit2D hit = Physics2D.Raycast(origin, direction.normalized, 100f, layerMask);

        if (hit.transform && hit.transform.CompareTag("Player")) {
            canShoot = false;

            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bullet.GetComponent<BulletP>().vel = (new Vector2(target.position.x, target.position.y) - new Vector2(transform.position.x, transform.position.y)).normalized * 4;
        }
    }
}
