using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

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
        bestDir = bestDir - ownPos;
        float bestDistance = bestDir.sqrMagnitude;
        for (int i = 1; i < players.Count; i++)
        {
            Vector2 newPos = players[i].transform.position;
            newPos = newPos - ownPos;
            float newDistance = newPos.sqrMagnitude;
            if (newDistance < bestDistance)
            {
                bestDistance = newDistance;
                bestDir = newPos;
            }
        }

        Mover.ConstantDirection = bestDir.normalized;
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
        => CreateInstance<FleeFromPlayersNode>();
}
