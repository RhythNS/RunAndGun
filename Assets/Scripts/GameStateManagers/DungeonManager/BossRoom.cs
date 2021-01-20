using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BossRoom : DungeonRoom
{
    public override bool EventOnRoomEntered => true;

    public override void OnAllPlayersEntered() {
        CloseDoors();
        SpawnBoss();

        AliveHealthDict.Instance.OnAllEnemiesDied += OnAllEnemiesDefeated;
        AliveHealthDict.Instance.OnAllPlayersDied += OnAllPlayersDied;
    }

    private void OnAllPlayersDied() {
        AliveHealthDict.Instance.OnAllEnemiesDied -= OnAllEnemiesDefeated;
        AliveHealthDict.Instance.OnAllPlayersDied -= OnAllPlayersDied;
    }

    private void OnAllEnemiesDefeated() {
        AliveHealthDict.Instance.OnAllEnemiesDied -= OnAllEnemiesDefeated;
        AliveHealthDict.Instance.OnAllPlayersDied -= OnAllPlayersDied;

        OpenDoors();
        SpawnLoot();
        AlreadyCleared = true;

        GameManager.OnRoomEventEnded();
    }

    private void SpawnBoss() {
        // TODO
    }

    private void SpawnLoot() {
        // TODO
    }
}
