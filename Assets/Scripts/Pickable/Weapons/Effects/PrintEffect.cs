﻿using UnityEngine;

/// <summary>
/// Prints a debug message when the bullet hit a targeted entity.
/// </summary>
[CreateAssetMenu(menuName = "Pickable/Weapon/Effect/PrintEffect")]
public class PrintEffect : Effect
{
    public override void OnHit(Weapon weapon, Health affecter, Health health)
    {
        Debug.Log((affecter != null ? affecter.gameObject.name : "Unknown") + " with weapon " 
            + weapon.name + " hit " + health.gameObject.name + " with a damage of " + weapon.BaseDamage);
    }
}
