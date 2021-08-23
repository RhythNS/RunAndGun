using UnityEngine;

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
