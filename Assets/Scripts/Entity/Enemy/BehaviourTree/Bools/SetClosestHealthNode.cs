using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

public class SetClosestHealthNode : BoolNode
{
    [SerializeField] private HealthValue targetHealth;
    [SerializeField] private bool targetPlayers = true;

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        SetClosestHealthNode schn = CreateInstance<SetClosestHealthNode>();
        schn.targetHealth = CloneValue(originalValueForClonedValue, targetHealth) as HealthValue;
        schn.targetPlayers = targetPlayers;
        return schn;
    }

    protected override void InnerReplaceValues(Dictionary<Value, Value> originalReplace)
    {
        if (originalReplace.ContainsKey(targetHealth))
            targetHealth = originalReplace[targetHealth] as HealthValue;
    }
    protected override bool InnerIsFulfilled()
    {
        List<Health> healths = targetPlayers ? AliveHealthDict.Instance.PlayerHealths : AliveHealthDict.Instance.EnemyHealths;

        if (healths.Count == 0)
            return false;

        Health bestHealth = healths[0];
        float bestDistance = (tree.AttachedBrain.transform.position - bestHealth.transform.position).sqrMagnitude;

        for (int i = 1; i < healths.Count; i++)
        {
            float newDistance = (tree.AttachedBrain.transform.position - healths[i].transform.position).sqrMagnitude;
            if (bestDistance > newDistance)
            {
                bestDistance = newDistance;
                bestHealth = healths[i];
            }
        }

        targetHealth.Set(bestHealth);
        return true;
    }
}
