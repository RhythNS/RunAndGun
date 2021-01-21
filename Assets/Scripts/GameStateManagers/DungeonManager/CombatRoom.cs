using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatRoom : DungeonRoom
{
    public override bool EventOnRoomEntered => true;
    [SerializeField] private EnemyObject[] enemiesToSpawn;

    public int ThreatLevel { get; set; }

    public override void OnAllPlayersEntered()
    {
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
            GameObject enemyObject = Instantiate(enemiesToSpawn[i].Prefab, MathUtil.RandomVector2(Border.min, Border.max), Quaternion.identity);
            enemyObject.GetComponent<Enemy>().Set(enemiesToSpawn[i]);
            NetworkServer.Spawn(enemyObject);
        }
    }

}
