using UnityEngine;

/// <summary>
/// Damages the player every tick.
/// </summary>
[CreateAssetMenu(menuName = "Pickable/Status/Poison")]
public class PoisonStatusEffect : StatusEffectAdapter
{
    /// <summary>
    /// The amount of damage done per tick.
    /// </summary>
    [SerializeField] private int damagePerTick;

    private ParticleSystem system;

    protected override void InnerOnPickup()
    {
        system = Instantiate(StatusEffectDict.Instance.PosionParticleSystem, OnHealth.transform);
    }

    public override void OnDrop()
    {
        Destroy(system.gameObject);
    }

    protected override void InnerOnTick()
    {
        if (IsServer)
            OnHealth.Damage(damagePerTick, Inflicter);
    }

    public override void OnEffectAlreadyInList(StatusEffect other)
    {
        PoisonStatusEffect otherPoison = other as PoisonStatusEffect;

        if (Id == other.Id)
        {
            NumberOfTicksRemaining = numberOfTicks;
            return;
        }

        if (damagePerTick * NumberOfTicksRemaining < otherPoison.numberOfTicks * otherPoison.damagePerTick)
        {
            NumberOfTicksRemaining = otherPoison.numberOfTicks;
            damagePerTick = otherPoison.damagePerTick;
        }
    }
}
