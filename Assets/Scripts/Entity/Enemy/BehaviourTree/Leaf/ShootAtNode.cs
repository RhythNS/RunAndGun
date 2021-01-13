using Rhyth.BTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shoots at a target. Returns false when the weapon is empty. Returns
/// true when the target died.
/// </summary>
public class ShootAtNode : BNodeAdapter
{
    public override int MaxNumberOfChildren => 0;

    [SerializeField] private HealthValue targetHealth;

    private bool weaponStartedFiring = false;

    private EquippedWeapon weapon;
    private Health health;

    public override void InnerBeginn()
    {
        weapon = tree.AttachedBrain.GetComponent<EquippedWeapon>();
        weaponStartedFiring = false;
    }

    public override void Update()
    {
        if (weaponStartedFiring == false)
        {
            health = targetHealth.Get();
            weaponStartedFiring = true;

            if (SetDirection())
                return;

            if (weapon.StartFire() == false)
            {
                CurrentStatus = Status.Failure;
                return;
            }

            return;
        }

        if (weapon.IsFiring == false)
        {
            CurrentStatus = Status.Failure;
            return;
        }

        SetDirection();
    }

    private bool SetDirection()
    {

        if (health == null || !health || health.Alive == false)
        {
            CurrentStatus = Status.Success;
            return true;
        }

        Vector2 healthPos = health.transform.position;
        weapon.SetDirection(healthPos - weapon.BulletSpawnPosition);
        return false;
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        ShootAtNode sat = CreateInstance<ShootAtNode>();
        sat.targetHealth = CloneValue(originalValueForClonedValue, targetHealth) as HealthValue;
        return sat;
    }

    protected override void InnerReplaceValues(Dictionary<Value, Value> originalReplace)
    {
        if (originalReplace.ContainsKey(targetHealth))
            targetHealth = originalReplace[targetHealth] as HealthValue;
    }
}
