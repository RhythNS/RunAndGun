using UnityEngine;

/// <summary>
/// A world effect that damages the entity when they enter or stay
/// in the world effect.
/// </summary>
[CreateAssetMenu(menuName = "World/WorldEffect/DamageWorldEffect")]
public class DamageWorldEffect : WorldEffect
{
    [SerializeField] private int damage;

    public override void OnEnter(Health toAffect, WorldEffectInWorld inWorld)
    {
        toAffect.Damage(damage, inWorld.Inflicter);
    }

    public override void OnTick(Health toAffect, WorldEffectInWorld inWorld)
    {
        toAffect.Damage(damage, inWorld.Inflicter);
    }
}
