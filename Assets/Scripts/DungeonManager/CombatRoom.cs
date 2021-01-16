using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatRoom : DungeonRoom
{
    public override bool EventOnRoomEntered => true;

    public override void OnAllPlayersEntered()
    {
        CloseDoors();
        // SpawnEnemies();
        GameManager.OnCombatStarted(bounds);

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

        GameManager.OnCombatEnded();
    }
}
