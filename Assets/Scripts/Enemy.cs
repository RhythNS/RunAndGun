using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private Weapon weapon;
    [SerializeField] private Player player;
    [SerializeField] private float moveSpeed;

    Rigidbody2D body;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        StartCoroutine(ShootAtPlayer());
        StartCoroutine(MoveAwayFromPlayer());
    }

    public void Set(Weapon weapon, Player player, float moveSpeed)
    {
        this.weapon = weapon;
        this.player = player;
        this.moveSpeed = moveSpeed;
    }

    private IEnumerator MoveAwayFromPlayer()
    {
        while (true)
        {
            Vector2 direction = transform.position - player.transform.position;
            if (direction.sqrMagnitude < 128.0f)
            {
                body.AddForce(direction.normalized * moveSpeed);
            }
            else
                body.AddForce(-direction.normalized * moveSpeed);


            yield return null;
        }
    }

    private IEnumerator ShootAtPlayer()
    {
        while (true)
        {
            Vector3 direction = player.transform.position - transform.position;
            direction.z = 0.0f;
            if (direction.sqrMagnitude < 200.0f)
            {
                weapon.Fire(gameObject, transform.position, direction.normalized);
                yield return new WaitForSeconds(weapon.ShootPerSecond);
            }
            else
                yield return new WaitForSeconds(0.5f);
        }
    }
}
