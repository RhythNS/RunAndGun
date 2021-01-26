using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

public class SetPathFromHealthNode : BoolNode
{
    [SerializeField] private HealthValue target;
    [SerializeField] private PathValue outPath;

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        SetPathFromHealthNode spfhn = CreateInstance<SetPathFromHealthNode>();
        spfhn.target = CloneValue(originalValueForClonedValue, target) as HealthValue;
        spfhn.outPath = CloneValue(originalValueForClonedValue, outPath) as PathValue;
        return spfhn;
    }

    protected override bool InnerIsFulfilled()
    {
        Health health;
        if (!target || !(health = target.Get()) || !health.Alive)
            return false;

        // DungeonDict.Instance.dungeon.TryFindPath()
        return false;
    }
}
