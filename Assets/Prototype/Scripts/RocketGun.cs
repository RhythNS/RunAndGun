using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New RocketGun", menuName = "Weapons/Rocket Gun")]
public class RocketGun : Weapon
{
    [SerializeField] private float aliveTime;
    [SerializeField] private float speed;
    [SerializeField] private float range;
    [SerializeField] private int damage;

    public override void Fire(GameObject shooter, Vector3 origin, Vector2 direction)
    {
        Instantiate(WeaponDict.Instance.misslePrefab, origin, Quaternion.identity).Set(shooter, direction * speed, damage, range, aliveTime);
    }
}
