using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets the given health value to the closest target.
/// </summary>
public class SetClosestHealthNode : BoolNode
{
    public override string StringToolTip => "Sets the given health value to the closest target. Returns true if another target was found.";

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

        Health ownHealth = tree.AttachedBrain.GetComponent<Health>();
        Health bestHealth = null;
        float bestDistance = 0.0f;

        int i;
        for (i = 0; i < healths.Count; i++)
        {
            if (healths[i] != ownHealth)
            {
                bestHealth = healths[i];
                bestDistance = (tree.AttachedBrain.transform.position - bestHealth.transform.position).sqrMagnitude;
                break;
            }
        }

        for (++i; i < healths.Count; i++)
        {
            if (healths[i] == ownHealth)
                continue;

            float newDistance = (tree.AttachedBrain.transform.position - healths[i].transform.position).sqrMagnitude;
            if (bestDistance > newDistance)
            {
                bestDistance = newDistance;
                bestHealth = healths[i];
            }
        }

        if (bestHealth == null)
            return false;

        targetHealth.Set(bestHealth);
        return true;
    }
}
