using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Pickable/Weapon/ShotModel/FullAutomatic")]
public class FullAutomaticModel : ShotModel
{
    [SerializeField] private float timeBetweenShots;

    public override IEnumerator Shoot(EquippedWeapon equipped)
    {
        while (!equipped.RequestStopFire && equipped.HasBulletsLeft)
        {
            equipped.Weapon.BulletSpawnModel.Shoot(equipped);
            yield return new WaitForSeconds(timeBetweenShots);
        }
    }
}
