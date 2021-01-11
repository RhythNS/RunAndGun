using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

public class IsHealthAliveNode : BoolNode
{
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
    protected override bool InnerIsFulfilled() => targetHealth.Get().Alive;

}
