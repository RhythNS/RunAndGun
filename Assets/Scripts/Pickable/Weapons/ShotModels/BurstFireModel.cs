using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pickable/Weapon/ShotModel/BurstFire")]
public class BurstFireModel : ShotModel
{
    [SerializeField] private float timeBetweenBursts;

    [SerializeField] private int shotsPerBurst;

    [SerializeField] private float timeBetweenShots;

    public override IEnumerator Shoot(EquippedWeapon equipped) {
        for (int i = 0; i < shotsPerBurst; i++) {
            if (!equipped.HasBulletsLeft)
                break;

            equipped.Weapon.BulletSpawnModel.Shoot(equipped);
            yield return new WaitForSeconds(timeBetweenShots);
        }

        yield return new WaitForSeconds(timeBetweenBursts);
        while (!equipped.RequestStopFire) yield return null;
    }
}
