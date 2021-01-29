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
        List<Vector2Int> enemySpawns = new List<Vector2Int>();

        int maxIterations = enemiesToSpawn.Length * 25;
        int iterations = 0;

        while (enemySpawns.Count < enemiesToSpawn.Length) {
            int rnd = Random.Range(0, walkableTiles.Count);
            Vector2Int pos = walkableTiles[rnd];

            bool found = false;
            for (int x = -2; x < 2; x++) {
                for (int y = -2; y < 2; y++) {
                    if (enemySpawns.Contains(new Vector2Int(x, y))) {
                        found = true;
                        break;
                    }
                }
            }

            if (!found) {
                Enemy.InstantiateAndSpawn(enemiesToSpawn[enemySpawns.Count], Border, new Vector3(pos.x, pos.y, 0f), Quaternion.identity);

                enemySpawns.Add(pos);
            }

            iterations++;
            if (iterations >= maxIterations)
                break;
        }
    }
}
