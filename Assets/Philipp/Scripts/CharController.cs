using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour
{
    [SerializeField]
    private GameObject bulletPrefab;

    private Rigidbody2D rb;
    private Vector2 vel;

    private float speed = 200;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Shoot();
        }
    }

    private void FixedUpdate() {
        MoveCharacter();
    }

    private void MoveCharacter() {
        vel = Vector2.zero;
        if (Input.GetKey(KeyCode.D))
            vel.x += 1;
        if (Input.GetKey(KeyCode.A))
            vel.x -= 1;
        if (Input.GetKey(KeyCode.W))
            vel.y += 1;
        if (Input.GetKey(KeyCode.S))
            vel.y -= 1;

        vel.Normalize();

        if (Input.GetKey(KeyCode.LeftShift))
            vel *= 2;
        vel *= speed * Time.fixedDeltaTime;

        rb.velocity = new Vector3(vel.x, vel.y, 0);
    }

    private void Shoot() {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10;

        Vector3 screenPos = Camera.main.ScreenToWorldPoint(mousePos);

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<BulletP>().vel = (new Vector2(screenPos.x, screenPos.y) - new Vector2(transform.position.x, transform.position.y)).normalized * 4;
        bullet.GetComponent<BulletP>().owner = true;
    }
}
