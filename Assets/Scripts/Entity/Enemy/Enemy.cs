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
    public static Enemy InstantiateAndSpawn(EnemyObject enemyObject, Rect roomBorder, Vector3 position, Quaternion quaternion)
    {
        GameObject gameObject = Instantiate(enemyObject.Prefab, position, quaternion);
        Enemy enemy = gameObject.GetComponent<Enemy>();
        enemy.Set(enemyObject, roomBorder);
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
    public StatusEffectList StatusEffectList { get; private set; }

    private void Awake()
    {
        Brain = GetComponent<Brain>(); // Should start disabled
        Health = GetComponent<Health>();
        EquippedWeapon = GetComponent<EquippedWeapon>();
        SmoothSync = GetComponent<SmoothSyncMirror>();
        StatusEffectList = GetComponent<StatusEffectList>();
    }

    public void Set(EnemyObject enemyObject, Rect roomBorder)
    {
        Brain.tree = enemyObject.BehaviourTree;
        EquippedWeapon.Swap(enemyObject.Weapon);

        Brain.BrainMover.meterPerSecond = enemyObject.Stats.metersPerSecond;
        Brain.BrainMover.RoomBounds = roomBorder;

        Health.Init(enemyObject.Stats.maxHealth);
    }

    public override void OnStartServer()
    {
        Brain.enabled = true;
    }
}
