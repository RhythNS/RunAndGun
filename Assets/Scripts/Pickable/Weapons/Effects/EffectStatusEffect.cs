using UnityEngine;

[CreateAssetMenu(menuName = "Pickable/Weapon/Effect/StatusEffect")]
public class EffectStatusEffect : Effect
{
    [SerializeField] private StatusEffect statusEffect;

    public override void OnHit(Weapon weapon, Health affecter, Health health)
    {
        health.StatusEffectList.ServerAdd(Instantiate(statusEffect), affecter);
    }
}
