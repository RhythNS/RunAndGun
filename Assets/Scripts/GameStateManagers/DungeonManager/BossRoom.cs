using MapGenerator;
using Mirror;
using UnityEngine;

public class BossRoom : DungeonRoom
{
    public override bool EventOnRoomEntered => false;
    public override RoomType RoomType => RoomType.Boss;

    public BossObject[] bossObjects;
    private ExtendedCoroutine enterAnimationCoroutine;
    private BossEnterAnimation enterAnimation;

    [Server]
    public void OnAllPlayersReadyToEnter()
    {
        GameObject[] objs = SpawnBoss();

        BossSpawnMessage bossSpawnMessage = new BossSpawnMessage
        {
            bossGameObjects = objs,
            id = this.id,
            animationType = bossObjects[0].AnimationType
        };

        NetworkServer.SendToAll(bossSpawnMessage);

        enterAnimation = BossEnterAnimation.AddAnimationType(gameObject, bossObjects[0].AnimationType);
        enterAnimationCoroutine = new ExtendedCoroutine(this, enterAnimation.PlayAnimation(objs[0], this), StartBossEncounter, true);
    }

    public void StartBossAnimation(BossSpawnMessage bossSpawnMessage)
    {
        if (!Player.LocalPlayer || Player.LocalPlayer.isServer)
            return;

        BossEnterAnimation bea = BossEnterAnimation.AddAnimationType(gameObject, bossSpawnMessage.animationType);
        enterAnimationCoroutine = new ExtendedCoroutine(this, bea.PlayAnimation(bossSpawnMessage.bossGameObjects[0], this), startNow: true);
    }

    private void StartBossEncounter()
    {
        CloseDoors();
        SpawnBoss();

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
        SpawnLoot();
        AlreadyCleared = true;

        GameManager.OnRoomEventEnded();
    }

    private GameObject[] SpawnBoss()
    {
        return null;
        // TODO
    }

    private void SpawnLoot()
    {
        // TODO
        // Spawn exit to next level
    }
}
