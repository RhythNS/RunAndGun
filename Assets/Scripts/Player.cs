using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;

    private Rigidbody2D body;
    private List<WeaponInWorld> closeByWeapons = new List<WeaponInWorld>();
    private Weapon weapon;
    private float weaponTimer;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Vector2 mov = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
            ) * speed;

        body.AddForce(mov);

        if (Input.GetKeyDown(KeyCode.E) && closeByWeapons.Count > 0)
        {
            if (weapon != null)
                Instantiate(WeaponDict.Instance.GetWeaponInWorldForWeapon(weapon)).Drop(this, weapon);

            weapon = closeByWeapons[0].PickUp();
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out WeaponInWorld weaponIn))
            closeByWeapons.Add(weaponIn);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out WeaponInWorld weaponIn))
            closeByWeapons.Remove(weaponIn);
    }
}
