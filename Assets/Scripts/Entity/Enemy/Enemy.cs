using Mirror;
using Smooth;
using UnityEngine;

/// <summary>
/// Enemies are entity that try to attack player entities.
/// </summary>
public class Enemy : Entity
{
    /// <summary>
    /// Helper method for creating and spawning a enemy.
    /// </summary>
    /// <param name="enemyObject">The scriptable object describing the enemy.</param>
    /// <param name="position">The position to where the enemy should be spawned to.</param>
    /// <param name="quaternion">The rotation of the enemy.</param>
    /// <returns>The spawned enemy.</returns>
    public static Enemy InstantiateAndSpawn(EnemyObject enemyObject, Vector3 position, Quaternion quaternion)
    {
        GameObject gameObject = Instantiate(enemyObject.Prefab, position, quaternion);
        Enemy enemy = gameObject.GetComponent<Enemy>();
        enemy.Set(enemyObject);
        NetworkServer.Spawn(gameObject);
        return enemy;
    }

    public override EntityType EntityType => EntityType.Enemy;

    /// <summary>
    /// Contains the BehaviourTree. This component should only be enabled on the Host.
    /// </summary>
    public Brain Brain { get; private set; }
    public Health Health { get; private set; }
    public EquippedWeapon EquippedWeapon { get; private set; }
    public SmoothSyncMirror SmoothSync { get; private set; }

    private void Awake()
    {
        Brain = GetComponent<Brain>(); // Should start disabled
        Health = GetComponent<Health>();
        EquippedWeapon = GetComponent<EquippedWeapon>();
        SmoothSync = GetComponent<SmoothSyncMirror>();
    }

    public void Set(EnemyObject enemyObject)
    {
        Brain.tree = enemyObject.BehaviourTree;
        EquippedWeapon.Swap(enemyObject.Weapon);
        // Set stats

        Brain.BrainMover.meterPerSecond = 2;
    }

    public override void OnStartServer()
    {
        Brain.enabled = true;
    }
}
