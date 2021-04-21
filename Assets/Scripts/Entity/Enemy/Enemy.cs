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

    public static Enemy InstantiateAndSpawn(BossObject bossObject, Rect roomBorder, Vector3 position, Quaternion quaternion)
    {
        GameObject gameObject = Instantiate(bossObject.EnemyObject.Prefab, position, quaternion);
        Enemy enemy = gameObject.GetComponent<Enemy>();
        enemy.IsBoss = true;
        enemy.Set(bossObject.EnemyObject, roomBorder);
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
    public EntityMaterialManager EntityMaterialManager { get; private set; }

    public bool IsBoss { get; private set; }

    public Weapon[] carryingWeapons;

    private void Awake()
    {
        Brain = GetComponent<Brain>(); // Should start disabled
        Health = GetComponent<Health>();
        EquippedWeapon = GetComponent<EquippedWeapon>();
        SmoothSync = GetComponent<SmoothSyncMirror>();
        StatusEffectList = GetComponent<StatusEffectList>();
        EntityMaterialManager = GetComponent<EntityMaterialManager>();
    }

    private void Update()
    {
        PositionConverter.AdjustZ(transform);
    }

    public void Set(EnemyObject enemyObject, Rect roomBorder)
    {
        if (IsBoss == false)
            EntityMaterialManager.PlaySpawnEffect();

        Brain.tree = enemyObject.BehaviourTree;

        carryingWeapons = enemyObject.Weapons;
        EquippedWeapon.Swap(enemyObject.Weapons[0]);

        Brain.BrainMover.meterPerSecond = enemyObject.Stats.metersPerSecond;
        Brain.BrainMover.RoomBounds = roomBorder;

        Health.Init(enemyObject.Stats.maxHealth);
    }

    public override void OnStartServer()
    {
        if (IsBoss == false)
            Brain.enabled = true;
    }
}
