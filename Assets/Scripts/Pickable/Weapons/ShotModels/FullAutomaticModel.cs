using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Pickable/Weapon/ShotModel/FullAutomatic")]
public class FullAutomaticModel : ShotModel
{
    [SerializeField] private float timeBetweenShots;

    public override IEnumerator Shoot(EquippedWeapon equipped)
    {
        while (!equipped.RequestStopFire && equipped.RemainingBullets > 0)
        {
            equipped.Weapon.BulletSpawnModel.Shoot(equipped);
            yield return new WaitForSeconds(timeBetweenShots);
        }
    }
}
