using UnityEngine;

/// <summary>
/// Damages another enity based on the base damage of the weapon.
/// </summary>
[CreateAssetMenu(menuName = "Pickable/Weapon/Effect/DamageEffect")]
public class DamageEffect : Effect
{
    public override void OnHit(Weapon weapon, Health affecter, Health health)
    {
        health.Damage(weapon.BaseDamage, affecter);
    }
}
