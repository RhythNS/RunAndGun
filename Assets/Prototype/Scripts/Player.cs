using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float dashDuration = 0.5f;
    [SerializeField] private float dashCooldown = 2.0f;

    private Rigidbody2D body;
    private List<WeaponInWorld> closeByWeapons = new List<WeaponInWorld>();
    private Weapon weapon;
    private float weaponTimer;
    private bool dashing = false;
    private float dashTimer = 0.0f;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (dashing == true)
            return;
        dashTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.E) && closeByWeapons.Count > 0)
        {
            if (weapon != null)
                Instantiate(WeaponDict.Instance.GetWeaponInWorldForWeapon(weapon)).Drop(this, weapon);

            weapon = closeByWeapons[0].PickUp();
        }

        if (dashTimer < 0.0f && Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartCoroutine(Dash());
        }

        weaponTimer -= Time.deltaTime;
        if (weapon == null || weaponTimer > 0)
            return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
            weapon.Fire(gameObject, transform.position, new Vector2(0.0f, 1.0f));
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            weapon.Fire(gameObject, transform.position, new Vector2(0.0f, -1.0f));
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            weapon.Fire(gameObject, transform.position, new Vector2(-1.0f, 0.0f));
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            weapon.Fire(gameObject, transform.position, new Vector2(1.0f, 0.0f));
        else
            return;

        weaponTimer = weapon.ShootPerSecond;
    }

    private void FixedUpdate()
    {
        if (dashing == true)
            return;

        Vector2 mov = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
            ) * speed;

        body.AddForce(mov);
    }

    private IEnumerator Dash()
    {
        gameObject.layer = 8;
        dashing = true;
        Vector2 vel = body.velocity * 2;
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        Color color = renderer.color;
        color.a = 0.5f;
        renderer.color = color;
        float timer = dashDuration;
        while(true)
        {
            timer -= Time.deltaTime;
            if (timer < 0.0f)
                break;
            body.velocity = vel;
            yield return null;
        }
        color.a = 1.0f;
        renderer.color = color;
        dashing = false;
        gameObject.layer = 0;
        dashTimer = dashCooldown;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out WeaponInWorld weaponIn))
            closeByWeapons.Add(weaponIn);
        else if (collision.gameObject.TryGetComponent(out InstantPickup pick))
            pick.PickUp(this);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out WeaponInWorld weaponIn))
            closeByWeapons.Remove(weaponIn);
    }
}
