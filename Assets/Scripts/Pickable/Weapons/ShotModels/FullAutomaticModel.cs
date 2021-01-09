using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Pickable/Weapon/ShotModel/FullAutomatic")]
public class FullAutomaticModel : ShotModel
{
    [SerializeField] private float timeBetweenShots;

    protected override IEnumerator InnerShoot(EquippedWeapon equipped)
    {
        while (!equipped.RequstStopFire && equipped.RemainingBullets > 0)
        {
            equipped.Weapon.BulletSpawnModel.Shoot(equipped);
            yield return new WaitForSeconds(timeBetweenShots);
        }
    }
}
