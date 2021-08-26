using UnityEngine;

/// <summary>
/// Dict for all universal objects that can spawn in a dungeon.
/// </summary>
public class DungeonRoomObjectDict : MonoBehaviour
{
    public static DungeonRoomObjectDict Instance { get; private set; }

    /// <summary>
    /// Placed before the doors on the boss room.
    /// </summary>
    public BossReadyZone BossReadyZone => bossReadyZone;
    [SerializeField] private BossReadyZone bossReadyZone;

    /// <summary>
    /// The zone that is used to advance to the next level after the boss was defeated.
    /// </summary>
    public AdvanceFloorZone AdvanceFloorZone => advanceFloorZone;
    [SerializeField] private AdvanceFloorZone advanceFloorZone;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("DungeonRoomObjectDict already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
