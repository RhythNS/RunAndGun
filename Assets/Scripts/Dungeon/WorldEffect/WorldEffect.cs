using UnityEngine;

/// <summary>
/// An effect that can be placed on the world.
/// </summary>
public class WorldEffect : Pickable
{
    public override PickableType PickableType => PickableType.WorldEffect;

    public enum TriggerType
    {
        Once, EveryReEnter, EveryTick
    }

    public TriggerType Type => type;
    [SerializeField] protected TriggerType type;

    /// <summary>
    /// Called when a health has entered the effect.
    /// </summary>
    /// <param name="toAffect">The health to be affected.</param>
    /// <param name="inWorld">The WorldEffectInWorld which holds additional data.</param>
    public virtual void OnEnter(Health toAffect, WorldEffectInWorld inWorld) { }

    /// <summary>
    /// Called each tick the health stood in the world effect. This is only called if the type is
    /// set to EveryTick.
    /// </summary>
    /// <param name="toAffect">The health to be affected.</param>
    /// <param name="inWorld">The WorldEffectInWorld which holds additional data.</param>
    public virtual void OnTick(Health toAffect, WorldEffectInWorld inWorld) { }

    /// <summary>
    /// Called when a health has exited the effect.
    /// </summary>
    /// <param name="toAffect">The health to be affected.</param>
    /// <param name="inWorld">The WorldEffectInWorld which holds additional data.</param>
    public virtual void OnExit(Health toAffect, WorldEffectInWorld inWorld) { }
}
