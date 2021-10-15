using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Flees from all alive Players.
/// </summary>
public class FleeFromPlayersNode : BNodeAdapter
{
    public override string StringToolTip => "Flees from all alive Players.\nOnly returns success when all Players died.";
    public override int MaxNumberOfChildren => 0;

    public override void Update()
    {
        List<Health> players = AliveHealthDict.Instance.PlayerHealths;
        if (players.Count == 0)
        {
            CurrentStatus = Status.Success;
            return;
        }
        Vector2 ownPos = Brain.transform.position;

        Vector2 bestDir = players[0].transform.position;
        bestDir = ownPos - bestDir;
        float bestDistance = bestDir.sqrMagnitude;
        for (int i = 1; i < players.Count; i++)
        {
            Vector2 newDir = players[i].transform.position;
            newDir = ownPos - newDir;
            float newDistance = newDir.sqrMagnitude;
            if (newDistance < bestDistance)
            {
                bestDistance = newDistance;
                bestDir = newDir;
            }
        }

        Mover.ConstantDirection = bestDir.normalized;
        Mover.ShouldMove = true;
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
        => CreateInstance<FleeFromPlayersNode>();
}
