using UnityEngine;

public class LayerDict : MonoBehaviour
{
    public static LayerDict Instance { get; private set; }

    [SerializeField] private int playerLayer;
    [SerializeField] private int enemyLayer;
    [SerializeField] private int dungeonRoomLayer;

    [SerializeField] private int bulletTargetPlayerOnly;
    [SerializeField] private int bulletTargetEnemyOnly;
    [SerializeField] private int bulletTargetPlayerAndEnemy;


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("LayerDict already in scene! Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public int GetBulletCollisionLayerMask()
    {
        // return objectLayer | someOtherLayerThatBulletsCanHit
        return ~(1 << enemyLayer | 1 << playerLayer | 1 << bulletTargetPlayerOnly | 1 << bulletTargetEnemyOnly | 1 << bulletTargetPlayerAndEnemy);
    }

    public int GetBulletLayer(EntityType entityType, TargetMode targetMode)
    {
        switch (entityType)
        {
            case EntityType.Player:
                switch (targetMode)
                {
                    case TargetMode.Allies:
                        return bulletTargetPlayerOnly;
                    case TargetMode.Enemies:
                        return bulletTargetEnemyOnly;
                    case TargetMode.Both:
                        return bulletTargetPlayerAndEnemy;
                    default:
                        throw new System.Exception("Target Mode: " + targetMode + " not implemented!");
                }
            case EntityType.Enemy:
                switch (targetMode)
                {
                    case TargetMode.Allies:
                        return bulletTargetEnemyOnly;
                    case TargetMode.Enemies:
                        return bulletTargetPlayerOnly;
                    case TargetMode.Both:
                        return bulletTargetPlayerAndEnemy;
                    default:
                        throw new System.Exception("Target Mode: " + targetMode + " not implemented!");
                }
            default:
                throw new System.Exception("Entity Type: " + entityType + " not implemented!");
        }
    }

    public int GetDungeonRoomLayer() => dungeonRoomLayer;

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
