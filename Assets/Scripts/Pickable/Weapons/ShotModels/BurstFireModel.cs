﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pickable/Weapon/ShotModel/BurstFire")]
public class BurstFireModel : ShotModel
{
    public override IEnumerator Shoot(Health shooter, EquippedWeapon equipped, Vector3 position, Vector2 direction)
    {
        throw new System.NotImplementedException();
    }
}