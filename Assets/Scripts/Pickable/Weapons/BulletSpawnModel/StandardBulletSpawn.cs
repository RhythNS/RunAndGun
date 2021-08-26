using UnityEngine;

/// <summary>
/// Spawns a single bullet.
/// </summary>
[CreateAssetMenu(menuName = "Pickable/Weapon/BulletSpawn/Standard")]
public class StandardBulletSpawn : BulletSpawnModel
{
    protected override void InnerShoot(EquippedWeapon weapon)
    {
        Vector2 vec = Quaternion.AngleAxis(GetAccuracyAngle(weapon.Weapon.Accuracy), Vector3.forward) * weapon.LocalDirection;
        weapon.SpawnNew(vec);
    }
}
