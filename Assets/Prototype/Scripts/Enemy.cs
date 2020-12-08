using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private Weapon weapon;
    [SerializeField] private Player player;
    [SerializeField] private float moveSpeed;

    [SerializeField] private bool shouldMove;

    private float rangeShootAtPlayer = 300.0f;
    Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        StartCoroutine(ShootAtPlayer());
        if (shouldMove == true)
            StartCoroutine(MoveAwayFromPlayer());
    }

    public void Set(Weapon weapon, Player player, float moveSpeed, bool shouldMove)
    {
        this.weapon = weapon;
        this.player = player;
        this.moveSpeed = moveSpeed;
        this.shouldMove = shouldMove;
    }

    private IEnumerator MoveAwayFromPlayer()
    {
        while (true)
        {
            Vector2 direction = transform.position - player.transform.position;
            float magnitude = direction.sqrMagnitude;

            if (magnitude > rangeShootAtPlayer)
                yield return new WaitForSeconds(Random.Range(1.0f, 1.1f));
            else if (magnitude < 128.0f)
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
