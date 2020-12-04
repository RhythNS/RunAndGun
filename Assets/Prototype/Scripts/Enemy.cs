using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private Weapon weapon;
    [SerializeField] private Player player;
    [SerializeField] private float moveSpeed;

    private float rangeShootAtPlayer = 200.0f;
    Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    public void Set(Weapon weapon, Player player, float moveSpeed, bool shouldMove)
    {
        this.weapon = weapon;
        this.player = player;
        this.moveSpeed = moveSpeed;

        StartCoroutine(ShootAtPlayer());
        if (shouldMove == true)
            StartCoroutine(MoveAwayFromPlayer());
        else
            rangeShootAtPlayer = float.MaxValue;
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
            if (direction.sqrMagnitude < rangeShootAtPlayer)
            {
                weapon.Fire(gameObject, transform.position, direction.normalized);
                yield return new WaitForSeconds(weapon.ShootPerSecond);
            }
            else
                yield return new WaitForSeconds(0.5f);
        }
    }
}
