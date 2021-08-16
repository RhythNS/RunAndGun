using MapGenerator;

/// <summary>
/// A dungeon room in which enemies can spawn in.
/// </summary>
public class CombatRoom : DungeonRoom
{
    public override bool EventOnRoomEntered => true;
    public override RoomType RoomType => RoomType.Combat;
 
    public int ThreatLevel { get; set; }
    
    public EnemyObject[] enemiesToSpawn;

    public override void OnAllPlayersEntered()
    {
        if (enemiesToSpawn.Length == 0)
            return;

        CloseDoors();
        SpawnEnemies(enemiesToSpawn);
        GameManager.OnRoomEventStarted();

        AliveHealthDict.Instance.OnAllEnemiesDied += OnAllEnemiesDefeated;
        AliveHealthDict.Instance.OnAllPlayersDied += OnAllPlayersDied;
    }

    private void OnAllPlayersDied()
    {
        AliveHealthDict.Instance.OnAllEnemiesDied -= OnAllEnemiesDefeated;
        AliveHealthDict.Instance.OnAllPlayersDied -= OnAllPlayersDied;
    }

    private void OnAllEnemiesDefeated()
    {
        AliveHealthDict.Instance.OnAllEnemiesDied -= OnAllEnemiesDefeated;
        AliveHealthDict.Instance.OnAllPlayersDied -= OnAllPlayersDied;

        OpenDoors();
        // SpawnLoot();
        AlreadyCleared = true;

        GameManager.OnRoomEventEnded();
    }
}
