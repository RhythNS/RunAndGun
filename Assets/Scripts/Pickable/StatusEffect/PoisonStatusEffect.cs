using UnityEngine;

[CreateAssetMenu(menuName = "Pickable/Status/Poison")]
public class PoisonStatusEffect : StatusEffectAdapter
{
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
            OnHealth.Damage(damagePerTick);
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
