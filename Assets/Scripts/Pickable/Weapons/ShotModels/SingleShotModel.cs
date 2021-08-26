using System.Collections;
using UnityEngine;

/// <summary>
/// Must be retriggered for every bullet shot.
/// </summary>
[CreateAssetMenu(menuName = "Pickable/Weapon/ShotModel/SingleShot")]
public class SingleShotModel : ShotModel
{
    [SerializeField] [Tooltip("How much time is in between individual rounds (in seconds).")]
    private float timeBetweenShots;

    public override IEnumerator Shoot(EquippedWeapon equipped)
    {
        equipped.Weapon.BulletSpawnModel.Shoot(equipped);
        yield return new WaitForSeconds(timeBetweenShots);
        while (!equipped.RequestStopFire) yield return null;
    }
}
