using Rhyth.BTree;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SetPathFromHealthNode : BNodeAdapter
{
    public override string StringToolTip => "Tries to generate a path from the entity to the health target.\nReturns success when a path was found. Returns false when the target is invalid, died or no path was found.";

    public override int MaxNumberOfChildren => 0;

    [SerializeField] private HealthValue target;
    [SerializeField] private PathValue outPath;

    private Task<List<Vector2>> task;

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

    public override void InnerRestart()
    {
        task = null;
    }

    public override void Update()
    {
        // Are we checking for a path?
        if (task == null)
        {
            Health health;
            if (!target || !(health = target.Get()) || !health.Alive) // Is the target still alive?
            {
                CurrentStatus = Status.Failure;
                return;
            }

            Vector2Int startPos = DungeonCreator.Instance.WorldPositionToTilePosition(Brain.transform.position);
            Vector2Int endPos = DungeonCreator.Instance.WorldPositionToTilePosition(health.transform.position);

            //task = Task<List<Vector2>>.Factory.StartNew(() => DebugPathFinder.Instance.TryFindPath(startPos, endPos));
            //Debug.LogWarning("SetPathFromHealthNode is running with debug path finder! This will only work in the test scene!");
            task = Task<List<Vector2>>.Factory.StartNew(() => DungeonDict.Instance.dungeon.TryFindPath(startPos, endPos));

            return;
        }

        if (task.IsCompleted == false) // Is the path calculated yet?
            return;

        List<Vector2> path = task.Result;

        if (path.Count == 0) // Was there no path found?
        {
            CurrentStatus = Status.Failure;
            outPath.Set(path);
            return;
        }

        // Check if the path is the same, if it is then return true without setting a reference to the new list.
        // This is because other nodes may use the list reference as a way to check if the path has changed or not.
        if (outPath.IsSame(path))
        {
            CurrentStatus = Status.Success;
            return;
        }

        // Path found and set
        outPath.Set(path);
        CurrentStatus = Status.Success;
    }
}
