using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Returns true if the given entity is still alive.
/// </summary>
public class IsInWeaponRange : BoolNode
{
    public override string StringToolTip => "Returns true if target is in range of current weapon.";

    [SerializeField] private HealthValue targetHealth;

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        IsInWeaponRange iiwr = CreateInstance<IsInWeaponRange>();
        iiwr.targetHealth = CloneValue(originalValueForClonedValue, targetHealth) as HealthValue;
        return iiwr;
    }

    protected override bool InnerIsFulfilled()
    {
        float range = tree.AttachedBrain.GetComponent<EquippedWeapon>().Weapon.Range;

        Vector2 targetPosition = targetHealth.Get().transform.position;
        Vector2 ownPosition = Brain.transform.position;

        Vector2 direction = ownPosition - targetPosition;

        return direction.magnitude < range;
    }
}
