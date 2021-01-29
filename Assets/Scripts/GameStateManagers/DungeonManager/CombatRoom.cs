using MapGenerator;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        SpawnEnemies();
        GameManager.OnRoomEventStarted(Border);

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

    protected void SpawnEnemies()
    {
        for (int i = 0; i < enemiesToSpawn.Length; i++)
        {
            Enemy.InstantiateAndSpawn(enemiesToSpawn[i], Border, MathUtil.RandomVector2(Border.min, Border.max), Quaternion.identity);
        }
    }

}
