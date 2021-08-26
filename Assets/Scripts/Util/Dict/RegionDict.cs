using UnityEngine;

/// <summary>
/// Dict for data for a region.
/// </summary>
public class RegionDict : MonoBehaviour
{
    public static RegionDict Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("RegionDict already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// The region that this dict is for.
    /// </summary>
    public Region Region => region;
    [SerializeField] private Region region;

    /// <summary>
    /// The items to spawn at the spawn point.
    /// </summary>
    public Pickable[] StartingRoomPickables => startingRoomPickables;
    [SerializeField] private Pickable[] startingRoomPickables;

    /// <summary>
    /// The items to spawn at looting rooms.
    /// </summary>
    public Pickable[] LootingRoomPickables => lootingRoomPickables;
    [SerializeField] private Pickable[] lootingRoomPickables;

    /// <summary>
    /// The bosses that can be spawned.
    /// </summary>
    public BossObject[] BossesToSpawn => bossesToSpawn;
    [SerializeField] private BossObject[] bossesToSpawn;

    /// <summary>
    /// The enemies that can be spawned.
    /// </summary>
    public EnemyObject[] EnemiesToSpawn => enemiesToSpawn;
    [SerializeField] private EnemyObject[] enemiesToSpawn;

    /// <summary>
    /// The Left-Right doors that spawn.
    /// </summary>
    public GameObject PrefabDoorLR => prefabDoorLR;
    [SerializeField] private GameObject prefabDoorLR;

    /// <summary>
    /// The Up-Down doors that spawn.
    /// </summary>
    public GameObject PrefabDoorUD => prefabDoorUD;
    [SerializeField] private GameObject prefabDoorUD;

    /// <summary>
    /// The tileset used by the dungeon generator.
    /// </summary>
    public Tileset Tileset => tileset;
    [SerializeField] Tileset tileset;

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
