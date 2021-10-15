using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Returns true if own health is above percentage.
/// </summary>
public class IsHealthAboveValueNode : BoolNode
{
    public override string StringToolTip => "Returns true if own health is above percentage.";

    [SerializeField] private float percentage;

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        IsHealthAboveValueNode ihav = CreateInstance<IsHealthAboveValueNode>();
        ihav.percentage = percentage;
        return ihav;
    }

    protected override bool InnerIsFulfilled()
    {
        Health health = tree.AttachedBrain.GetComponent<Health>();
        
        return (float)health.Current / health.Max > percentage;
    }
}
