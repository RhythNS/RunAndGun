using UnityEngine;

/// <summary>
/// Adapter for adding a status effect to a bullet that is applied to another entity on hit.
/// </summary>
[CreateAssetMenu(menuName = "Pickable/Weapon/Effect/StatusEffect")]
public class EffectStatusEffect : Effect
{
    [SerializeField] private StatusEffect statusEffect;

    public override void OnHit(Weapon weapon, Health affecter, Health health)
    {
        health.StatusEffectList.ServerAdd(Instantiate(statusEffect), affecter);
    }
}
