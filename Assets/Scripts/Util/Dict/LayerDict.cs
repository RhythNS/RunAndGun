using UnityEngine;

/// <summary>
/// Dict for information about the layers.
/// </summary>
public class LayerDict : MonoBehaviour
{
    public static LayerDict Instance { get; private set; }

    [SerializeField] private int playerLayer;
    [SerializeField] private int enemyLayer;
    [SerializeField] private int downedPlayerLayer;
    [SerializeField] private int pickableLayer;
    [SerializeField] private int dungeonRoomLayer;

    [SerializeField] private int bulletTargetPlayerOnly;
    [SerializeField] private int bulletTargetEnemyOnly;
    [SerializeField] private int bulletTargetPlayerAndEnemy;

    [SerializeField] private int unhittableLayer;


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

    /// <summary>
    /// Gets the bit mask of everything that the bullet can hit.
    /// </summary>
    public int GetBulletCollisionLayerMask()
    {
        // return objectLayer | someOtherLayerThatBulletsCanHit
        return ~(1 << enemyLayer | 1 << playerLayer | 1 << bulletTargetPlayerOnly | 1 << bulletTargetEnemyOnly | 1 << bulletTargetPlayerAndEnemy
            | 1 << dungeonRoomLayer | 1 << pickableLayer | 1 << downedPlayerLayer | 1 << unhittableLayer);
    }

    /// <summary>
    /// Gets the layer a bullet should be spawned on.
    /// </summary>
    /// <param name="entityType">The entity type that spawned the bullet.</param>
    /// <param name="targetMode">The target mode of the gun.</param>
    /// <returns>The layer.</returns>
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

    /// <summary>
    /// Gets the player layer.
    /// </summary>
    public int GetPlayerLayer() => playerLayer;

    /// <summary>
    /// Gets the enemy layer.
    /// </summary>
    public int GetEnemyLayer() => enemyLayer;

    /// <summary>
    /// Gets the downed player layer.
    /// </summary>
    public int GetDownedPlayerLayer() => downedPlayerLayer;

    /// <summary>
    /// Gets the dungeon layer.
    /// </summary>
    public int GetDungeonRoomLayer() => dungeonRoomLayer;

    /// <summary>
    /// Gets the pickable layer.
    /// </summary>
    public int GetPickableLayer() => pickableLayer;

    /// <summary>
    /// Gets the unhittable layer.
    /// </summary>
    public int GetUnhittableLayer() => unhittableLayer;

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
