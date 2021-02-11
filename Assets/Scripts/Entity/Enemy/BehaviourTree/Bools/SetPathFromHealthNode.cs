using Rhyth.BTree;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class SetPathFromHealthNode : BoolNode
{
    public override string StringToolTip => "Tries to generate a path from the entity to the health target.\nReturns success when a path was found. Returns false when the target is invalid, died or no path was found.";

    [SerializeField] private HealthValue target;
    [SerializeField] private PathValue outPath;

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        SetPathFromHealthNode spfhn = CreateInstance<SetPathFromHealthNode>();
        spfhn.target = CloneValue(originalValueForClonedValue, target) as HealthValue;
        spfhn.outPath = CloneValue(originalValueForClonedValue, outPath) as PathValue;
        return spfhn;
    }

    protected override void InnerReplaceValues(Dictionary<Value, Value> originalReplace)
    {
        if (originalReplace.TryGetValue(target, out Value replaceTarget))
            target = replaceTarget as HealthValue;
        if (originalReplace.TryGetValue(outPath, out Value replacePath))
            outPath = replacePath as PathValue;
    }

    protected override bool InnerIsFulfilled()
    {
        Health health;
        if (!target || !(health = target.Get()) || !health.Alive)
            return false;

        Vector2Int startPos = DungeonCreator.Instance.WorldPositionToTilePosition(Brain.transform.position);
        Vector2Int endPos = DungeonCreator.Instance.WorldPositionToTilePosition(health.transform.position);

        /*
        if (DungeonDict.Instance.dungeon.TryFindPath(startPos, endPos, out List<Vector2> path) == false)
            return false;
         */

        bool stop = false;
        Stopwatch sw = Stopwatch.StartNew();
        if (DebugPathFinder.Instance.TryFindPath(startPos, endPos, out List<Vector2> path) == false)
            stop = true;
        UnityEngine.Debug.Log("Pathfinder took " + (sw.ElapsedMilliseconds).ToString() + "ms");
        sw.Stop();
        if (stop)
            return false;

        // Check if the path is the same, if it is then return true without setting a reference to the new list.
        // This is because other nodes may use the list reference as a way to check if the path has changed or not.
        if (outPath.IsSame(path))
            return true;

        outPath.Set(path);

        return true;
    }
}
