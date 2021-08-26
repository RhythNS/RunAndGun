using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shoots at a target with movement prediction. Returns false when the weapon is empty. Returns
/// true when the target died.
/// </summary>
public class ShootAtPredictNode : ShootAtNode
{
    public override string StringToolTip => "Shoots at a target with movement prediction.\nReturns failure when the weapon is empty. Returns success when the target died.";

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

        Vector2 targetPosition = health.transform.position;
        Vector2 targetVelocity = health.GetComponent<Rigidbody2D>().velocity;

        float distance = ((Vector2)Brain.transform.position - targetPosition).magnitude;

        Vector2 predictedPosition = targetPosition + targetVelocity * (distance / weapon.Weapon.Speed);

        weapon.SetDirection(predictedPosition - weapon.BulletSpawnPosition);
        return false;
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        ShootAtPredictNode sat = CreateInstance<ShootAtPredictNode>();
        sat.targetHealth = CloneValue(originalValueForClonedValue, targetHealth) as HealthValue;
        return sat;
    }
}
