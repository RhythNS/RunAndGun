using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pickable/Weapon/BulletSpawn/Standard")]
public class StandardBulletSpawn : BulletSpawnModel
{
    protected override void InnerShoot(EquippedWeapon weapon) {
        Vector2 vec = Quaternion.AngleAxis(GetAccuracyAngle(weapon.Weapon.Accuracy), Vector3.forward) * weapon.LocalDirection;
        weapon.SpawnNew(vec);
    }
}
