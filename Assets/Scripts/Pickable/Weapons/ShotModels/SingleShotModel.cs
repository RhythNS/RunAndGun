using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Pickable/Weapon/ShotModel/SingleShot")]
public class SingleShotModel : ShotModel
{
    [SerializeField] private float timeBetweenShots;

    public override IEnumerator Shoot(EquippedWeapon equipped)
    {
        equipped.Weapon.BulletSpawnModel.Shoot(equipped);
        yield return new WaitForSeconds(timeBetweenShots);
        while (!equipped.RequstStopFire) yield return null;
    }
}
