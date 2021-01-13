using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

public class CanSeeNode : BoolNode
{
    [SerializeField] private HealthValue otherHealth;
    [SerializeField] private LayerMask layerMask;

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
        throw new System.Exception("Not yet implemented!");
    }
}
