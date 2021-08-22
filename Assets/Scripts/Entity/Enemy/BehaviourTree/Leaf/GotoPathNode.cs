using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Goes on a specified path.
/// </summary>
public class GotoPathNode : BNodeAdapter
{
    public override int MaxNumberOfChildren => 0;

    [SerializeField] private PathValue path;
    private int atNode;
    private List<Vector2> gottenPath;

    public override string StringToolTip => "Goes on a specified path.";

    public override void InnerBeginn()
    {
        gottenPath = path.Get();
        atNode = 0;
        if (path != null && gottenPath.Count > 0)
            Mover.Destination = gottenPath[0];
    }

    public override void InnerRestart()
    {
        gottenPath = null;
    }

    public override void Update()
    {
        if (gottenPath == null || gottenPath.Count == 0)
        {
            CurrentStatus = Status.Failure;
            return;
        }

        if (gottenPath != path.Get()) // Path has changed. Restart the node
            InnerBeginn();

        switch (Mover.State)
        {
            case BrainMover.PathState.InProgress:
                Mover.ShouldMove = true;
                break;

            case BrainMover.PathState.Reached:
                if (++atNode >= gottenPath.Count)
                {
                    CurrentStatus = Status.Success;
                    return;
                }
                Mover.Destination = gottenPath[atNode];
                break;

            case BrainMover.PathState.Unreachable:
                CurrentStatus = Status.Failure;
                break;

            case BrainMover.PathState.ConstantDirection:
                break;
        }
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        GotoPathNode gpn = CreateInstance<GotoPathNode>();
        gpn.path = CloneValue(originalValueForClonedValue, path) as PathValue;
        return gpn;
    }

    protected override void InnerReplaceValues(Dictionary<Value, Value> originalReplace)
    {
        if (originalReplace.TryGetValue(path, out Value replacePath))
            path = replacePath as PathValue;
    }
}
