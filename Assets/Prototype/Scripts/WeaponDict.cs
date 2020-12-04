using UnityEngine;

public class WeaponDict : MonoBehaviour
{
    public static WeaponDict Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public WeaponInWorld simpleGun, rocketGun, sword, aoeWeapon, shotGun;

    public Bullet bulletPrefab;
    public Missle misslePrefab;
    public Explosion explosionPrefab;
    public SwordSwingWave wavePrefab;
    public AOEBullet aoeBullet;

    public WeaponInWorld GetWeaponInWorldForWeapon(Weapon weapon)
    {
        if (weapon is SimpleGun)
            return simpleGun;
        else if (weapon is RocketGun)
            return rocketGun;
        else if (weapon is Sword)
            return sword;
        else if (weapon is AOEGun)
            return aoeWeapon;
        else if (weapon is ShotGun)
            return shotGun;

        throw new System.Exception("Gun " + weapon.name + " not in dict!");
    }
}
