using UnityEngine;

[CreateAssetMenu(menuName = "Pickable/Weapon/Effect/DamageEffect")]
public class DamageEffect : Effect
{
    public override void OnHit(Weapon weapon, Health health)
    {
        health.Damage(weapon.BaseDamage);
    }
}
