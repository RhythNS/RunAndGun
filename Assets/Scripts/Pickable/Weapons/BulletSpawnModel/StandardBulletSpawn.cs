using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pickable/Weapon/BulletSpawn/Standard")]
public class StandardBulletSpawn : BulletSpawnModel
{
    protected override void InnerShoot(EquippedWeapon weapon)
    {
        Bullet.Set(weapon);
    }
}
