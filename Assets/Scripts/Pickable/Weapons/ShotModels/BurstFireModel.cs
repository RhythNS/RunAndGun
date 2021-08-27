﻿using System.Collections;
using UnityEngine;

/// <summary>
/// Fires a specific amount of bullets before the entity must retrigger the shoot method.
/// </summary>
[CreateAssetMenu(menuName = "Pickable/Weapon/ShotModel/BurstFire")]
public class BurstFireModel : ShotModel
{
    /// <summary>
    /// How much time is in between the bursts (in seconds).
    /// </summary>
    [SerializeField] [Tooltip("How much time is in between the bursts (in seconds).")]
    private float timeBetweenBursts;

    /// <summary>
    /// How many shots are in one burst.
    /// </summary>
    [SerializeField] [Tooltip("How many shots are in one burst.")]
    private int shotsPerBurst;

    /// <summary>
    /// How much time is in between individual rounds (in seconds).
    /// </summary>
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
