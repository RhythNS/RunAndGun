using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Pickable/Weapon/ShotModel/SingleShot")]
public class SingleShotModel : ShotModel
{
    public override IEnumerator Shoot(Health shooter, EquippedWeapon equipped, Vector3 position, Vector2 direction)
    {
        equipped.OnFiredSingleShot();
        equipped.Weapon.BulletSpawnModel.Shoot(shooter, equipped, position, direction);
        yield break;
    }
}
