/// <summary>
/// Helper class that can be inherrited from, if not every method of status effect
/// is needed.
/// </summary>
public abstract class StatusEffectAdapter : StatusEffect
{
    public override void OnDrop() { }

    protected override void InnerOnPickup() { }

    protected override void InnerOnTick() { }
}
