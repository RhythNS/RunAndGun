using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

public class StayDistanceFromPlayersNode : BNodeAdapter
{
    public override int MaxNumberOfChildren => 0;

    [SerializeField] private HealthValue target;
    [SerializeField] private float distancePercent = 0.8f;

    private Health health;
    private float prefDistance;

    public override void InnerBeginn()
    {
        health = target.Get();
        prefDistance = Brain.GetComponent<Enemy>().EquippedWeapon.Weapon.Range * distancePercent;
    }

    public override void Update()
    {
        if (EnemyNodeUtil.TargetAlive(health) == false)
        {
            CurrentStatus = Status.Success;
            return;
        }
        Vector2 ownPos = Brain.transform.position;
        Vector2 dir = (ownPos - ((Vector2)health.transform.position)).normalized;
        Mover.Destination = ownPos + (dir * prefDistance);
        Mover.ShouldMove = true;
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        StayDistanceFromPlayersNode sdfpn = CreateInstance<StayDistanceFromPlayersNode>();
        sdfpn.target = CloneValue(originalValueForClonedValue, target) as HealthValue;
        sdfpn.distancePercent = distancePercent;
        return sdfpn;
    }

    protected override void InnerReplaceValues(Dictionary<Value, Value> originalReplace)
    {
        if (originalReplace.ContainsKey(target))
            target = originalReplace[target] as HealthValue;
    }
}
