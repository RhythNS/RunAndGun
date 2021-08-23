using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

public class StayInRangeOfPlayerNode : BNodeAdapter
{
    public override int MaxNumberOfChildren => 0;

    public override string StringToolTip => "Tries to stay within the specified min and max distance from the closest player.";

    [SerializeField] private HealthValue target;
    [SerializeField] private float minDistance = 6.0f;
    [SerializeField] private float maxDistance = 8.0f;

    private Health health;

    public override void InnerBeginn()
    {
        health = target.Get();
    }

    public override void Update()
    {
        if (EnemyNodeUtil.TargetAlive(health) == false)
        {
            CurrentStatus = Status.Success;
            return;
        }

        Vector2 ownPos = Brain.transform.position;
        Vector2 targetPos = health.transform.position;
        Vector2 difference = ownPos - targetPos;

        float sqrDistance = difference.sqrMagnitude;
        if (sqrDistance < minDistance * minDistance)
        {
            float distanceFactor = minDistance + (maxDistance - minDistance) * 0.75f;
            Mover.Destination = targetPos + (difference.normalized * distanceFactor);
        }
        else if (sqrDistance > maxDistance * maxDistance)
        {
            float distanceFactor = minDistance + (maxDistance - minDistance) * 0.25f;
            Mover.Destination = targetPos + (difference.normalized * distanceFactor);
        }

        if (Mathf.Abs(Mover.Destination.sqrMagnitude - ownPos.sqrMagnitude) > 0.0625f)
        {
            Mover.ShouldMove = true;
        }
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        StayInRangeOfPlayerNode sdfpn = CreateInstance<StayInRangeOfPlayerNode>();
        sdfpn.target = CloneValue(originalValueForClonedValue, target) as HealthValue;
        sdfpn.minDistance = minDistance;
        sdfpn.maxDistance = maxDistance;
        return sdfpn;
    }

    protected override void InnerReplaceValues(Dictionary<Value, Value> originalReplace)
    {
        if (originalReplace.ContainsKey(target))
            target = originalReplace[target] as HealthValue;
    }
}
