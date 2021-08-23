using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Returns true if the given entity is still alive.
/// </summary>
public class IsHealthAboveValue : BoolNode
{
    public override string StringToolTip => "Returns true if own health is above percentage.";

    [SerializeField] private float percentage;

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        IsHealthAboveValue ihav = CreateInstance<IsHealthAboveValue>();
        ihav.percentage = percentage;
        return ihav;
    }

    protected override bool InnerIsFulfilled()
    {
        Health health = tree.AttachedBrain.GetComponent<Health>();
        
        return health.Current / health.Max > percentage;
    }
}
