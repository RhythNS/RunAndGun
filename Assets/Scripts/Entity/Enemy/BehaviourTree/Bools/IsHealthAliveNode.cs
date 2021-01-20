using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

public class IsHealthAliveNode : BoolNode
{
    public override string StringToolTip => "This node is a BoolNode!\nReturns true if the given entity is still alive.";

    [SerializeField] private HealthValue targetHealth;

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        IsHealthAliveNode ihan = CreateInstance<IsHealthAliveNode>();
        ihan.targetHealth = CloneValue(originalValueForClonedValue, targetHealth) as HealthValue;
        return ihan;
    }

    protected override void InnerReplaceValues(Dictionary<Value, Value> originalReplace)
    {
        if (originalReplace.ContainsKey(targetHealth))
            targetHealth = originalReplace[targetHealth] as HealthValue;
    }
    protected override bool InnerIsFulfilled() => targetHealth ? targetHealth.Get().Alive : false;

}
