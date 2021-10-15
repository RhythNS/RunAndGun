using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Returns true if own health is below percentage.
/// </summary>
public class IsHealthBelowValueNode : BoolNode
{
    public override string StringToolTip => "Returns true if own health is below percentage.";

    [SerializeField] private float percentage;

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        IsHealthBelowValueNode ihbv = CreateInstance<IsHealthBelowValueNode>();
        ihbv.percentage = percentage;
        return ihbv;
    }

    protected override bool InnerIsFulfilled()
    {
        Health health = Brain.GetComponent<Health>();
        
        return (float)health.Current / health.Max < percentage;
    }
}
