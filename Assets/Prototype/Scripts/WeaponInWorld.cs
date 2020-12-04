using UnityEngine;

public class WeaponInWorld : MonoBehaviour
{
    [SerializeField] private Weapon weapon;

    public Weapon PickUp()
    {
        Destroy(gameObject);
        return weapon;
    }

    public void Drop(Player player, Weapon weapon)
    {
        transform.position = player.transform.position;
        this.weapon = weapon;
    }
}
