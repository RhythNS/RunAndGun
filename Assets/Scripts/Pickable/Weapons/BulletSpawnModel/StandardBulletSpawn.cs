using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pickable/Weapon/BulletSpawn/Standard")]
public class StandardBulletSpawn : BulletSpawnModel
{
    public override void Shoot(Health shooter, EquippedWeapon weapon, Vector3 position, Vector2 direction)
    {
        Bullet.Set(shooter, weapon, position, direction);
    }
}
