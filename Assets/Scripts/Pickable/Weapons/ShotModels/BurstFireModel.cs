using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Pickable/Weapon/ShotModel/BurstFire")]
public class BurstFireModel : ShotModel
{
    [SerializeField] [Tooltip("How much time is in between the bursts (in seconds).")]
    private float timeBetweenBursts;

    [SerializeField] [Tooltip("How many shots are in one burst.")]
    private int shotsPerBurst;

    [SerializeField] [Tooltip("How much time is in between individual rounds (in seconds).")]
    private float timeBetweenShots;

    public override IEnumerator Shoot(EquippedWeapon equipped)
    {
        for (int i = 0; i < shotsPerBurst; i++)
        {
            if (!equipped.HasBulletsLeft)
                break;

            equipped.Weapon.BulletSpawnModel.Shoot(equipped);
            yield return new WaitForSeconds(timeBetweenShots);
        }

        yield return new WaitForSeconds(timeBetweenBursts);
        while (!equipped.RequestStopFire) yield return null;
    }
}
