using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pickable/Weapon/BulletSpawn/Spray")]
public class SprayBulletSpawn : BulletSpawnModel
{
    public override void Shoot(Health shooter, Bullet bullet, Vector3 position, Vector2 direction)
    {
        throw new System.NotImplementedException();
    }
}
