using UnityEngine;

/// <summary>
/// Damages the player once when picked up.
/// </summary>
[CreateAssetMenu(menuName = "Pickable/Status/Damage")]
public class DamageStatusEffect : StatusEffectAdapter
{
    [SerializeField] private int damage;

    public override void OnEffectAlreadyInList(StatusEffect other)
    {
        // Should not happen
    }

    protected override void InnerOnPickup()
    {
        OnHealth.Damage(damage, Inflicter);
    }
}
