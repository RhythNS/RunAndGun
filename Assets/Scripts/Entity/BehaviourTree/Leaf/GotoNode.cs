using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Goes to a specified location.
/// </summary>
public class GotoNode : BNodeAdapter
{
    public override int MaxNumberOfChildren => 0;

    public override string StringToolTip => "Goes to a specified location.";

    [SerializeField] private Vector2 destPos;

    public override void InnerBeginn()
    {
        tree.AttachedBrain.BrainMover.Destination = destPos;
    }

    public override void Update()
    {
        switch (Mover.State)
        {
            case BrainMover.PathState.Reached:
                CurrentStatus = Status.Success;
                break;
            case BrainMover.PathState.Unreachable:
                CurrentStatus = Status.Failure;
                break;
        }
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        GotoNode cloned = CreateInstance<GotoNode>();
        cloned.destPos = destPos;
        return cloned;
    }
}
