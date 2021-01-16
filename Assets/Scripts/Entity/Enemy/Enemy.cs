using Smooth;

public class Enemy : Entity
{
    public override EntityType EntityType => EntityType.Enemy;

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
    }

    public override void OnStartServer()
    {
        Brain.enabled = true;
    }
}
