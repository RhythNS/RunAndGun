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
    public override string StringToolTip => "Shoots at a target.\nReturns failure when the weapon is empty. Returns success when the target died.";

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
                // Weapon can not fire for whatever reason.
                CurrentStatus = Status.Failure;
                return;
            }
            else
            {
                // Check, if weapon uses some kind of "single trigger pull" weapon and stop firing
                System.Type shotModel = weapon.Weapon.ShotModel.GetType();
                if (shotModel == typeof(SingleShotModel) || shotModel == typeof(BurstFireModel))
                {
                    weapon.StopFire();
                }
            }

            return;
        }

        // Weapon is not firing anymore and entity.
        if (weapon.IsFiring == false)
        {
            CurrentStatus = Status.Failure;
            return;
        }

        SetDirection();
    }

    /// <summary>
    /// Sets the direction of the EquippedWeapon.
    /// </summary>
    /// <returns>True if the target was killed. False otherwise.</returns>
    private bool SetDirection()
    {
        if (EnemyNodeUtil.TargetAlive(health) == false)
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
