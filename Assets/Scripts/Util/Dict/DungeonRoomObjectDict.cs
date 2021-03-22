using UnityEngine;

public class DungeonRoomObjectDict : MonoBehaviour
{
    public static DungeonRoomObjectDict Instance { get; private set; }

    public BossReadyZone BossReadyZone => bossReadyZone;
    [SerializeField] private BossReadyZone bossReadyZone;

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
