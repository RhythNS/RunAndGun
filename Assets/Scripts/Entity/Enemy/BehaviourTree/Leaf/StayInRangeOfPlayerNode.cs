using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tries to stay within the specified min and max distance from the closest player.
/// </summary>
public class StayInRangeOfPlayerNode : BNodeAdapter
{
    public override int MaxNumberOfChildren => 0;

    public override string StringToolTip => "Tries to stay within the specified min and max distance from the closest player.";

    [SerializeField] private HealthValue target;
    [SerializeField] private FloatValue minDistance;
    [SerializeField] private FloatValue maxDistance;

    private Health health;
    public float min, max, distanceFactor;
    private const float DISTANCE_TOLERANCE = 0.25f * 0.25f;

    public override void InnerBeginn()
    {
        health = target.Get();

        min = minDistance.Get();
        max = maxDistance.Get();

        distanceFactor = (max + min) * 0.5f;
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
        if (sqrDistance < min * min || sqrDistance > max * max)
            Mover.Destination = targetPos + (difference.normalized * distanceFactor);

        if (Mathf.Abs(Mover.Destination.sqrMagnitude - ownPos.sqrMagnitude) > DISTANCE_TOLERANCE)
            Mover.ShouldMove = true;
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        StayInRangeOfPlayerNode sdfpn = CreateInstance<StayInRangeOfPlayerNode>();
        sdfpn.target = CloneValue(originalValueForClonedValue, target) as HealthValue;
        sdfpn.minDistance = CloneValue(originalValueForClonedValue, minDistance) as FloatValue;
        sdfpn.maxDistance = CloneValue(originalValueForClonedValue, maxDistance) as FloatValue;
        return sdfpn;
    }

    protected override void InnerReplaceValues(Dictionary<Value, Value> originalReplace)
    {
        if (originalReplace.ContainsKey(target))
            target = originalReplace[target] as HealthValue;
    }
}
