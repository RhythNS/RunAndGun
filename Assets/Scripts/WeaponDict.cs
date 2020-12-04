using UnityEngine;

public class WeaponDict : MonoBehaviour
{
    public static WeaponDict Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public WeaponInWorld simpleGun, rocketGun;

    public Bullet bulletPrefab;
    public Missle misslePrefab;
    public Explosion explosionPrefab;

    public WeaponInWorld GetWeaponInWorldForWeapon(Weapon weapon)
    {
        if (weapon is SimpleGun)
            return simpleGun;
        if (weapon is RocketGun)
            return rocketGun;

        throw new System.Exception("Gun " + weapon.name + " not in dict!");
    }
}
