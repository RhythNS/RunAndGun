using UnityEngine;

/// <summary>
/// A World effect that adds a Status effect to the target.
/// </summary>
[CreateAssetMenu(menuName = "World/WorldEffect/StatusWorldEffect")]
public class StatusEffectWorldEffect : WorldEffect
{
    /// <summary>
    /// Effects to be added to the target.
    /// </summary>
    [SerializeField] private StatusEffect[] effects;

    public override void OnEnter(Health toAffect, WorldEffectInWorld inWorld)
    {
        for (int i = 0; i < effects.Length; i++)
            toAffect.StatusEffectList.Add(Instantiate(effects[i]), inWorld.Inflicter);
    }

    public override void OnTick(Health toAffect, WorldEffectInWorld inWorld)
    {
        for (int i = 0; i < effects.Length; i++)
            toAffect.StatusEffectList.Add(Instantiate(effects[i]), inWorld.Inflicter);
    }
}
