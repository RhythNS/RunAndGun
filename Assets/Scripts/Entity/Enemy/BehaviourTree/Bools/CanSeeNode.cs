using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks to see if an entity can see another entity.
/// </summary>
public class CanSeeNode : BoolNode
{
    public override string StringToolTip => "Returns success if the entity can see a other given entity.";

    [SerializeField] private HealthValue otherHealth;

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        CanSeeNode csn = CreateInstance<CanSeeNode>();
        csn.otherHealth = CloneValue(originalValueForClonedValue, otherHealth) as HealthValue;
        return csn;
    }

    protected override void InnerReplaceValues(Dictionary<Value, Value> originalReplace)
    {
        if (originalReplace.ContainsKey(otherHealth))
            otherHealth = originalReplace[otherHealth] as HealthValue;
    }

    protected override bool InnerIsFulfilled()
    {
        Vector2 ownPos, healthPos, dir;
        Health other;

        if (!otherHealth || !(other = otherHealth.Get()) || !other.Alive)
            return false;

        healthPos = other.transform.position;
        ownPos = Brain.transform.position;

        dir = healthPos - ownPos;

        RaycastHit2D hit = Physics2D.Raycast(ownPos, dir, dir.magnitude, LayerDict.Instance.GetBulletCollisionLayerMask());

        return hit.collider == null;
    }
}
