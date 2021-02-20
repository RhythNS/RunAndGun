using MapGenerator;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom : DungeonRoom
{
    public override bool EventOnRoomEntered => false;
    public override RoomType RoomType => RoomType.Boss;

    public Enemy[] SpawnedEnemies { get; private set; }

    public BossObject[] bossObjects;

    private ExtendedCoroutine enterAnimationCoroutine;
    private BossEnterAnimation enterAnimation;

    [Server]
    public void OnAllPlayersReadyToEnter()
    {
        GameObject[] objs = SpawnBoss();
        CloseDoors();

        BossSpawnMessage bossSpawnMessage = new BossSpawnMessage
        {
            bossGameObjects = objs,
            id = id,
            animationType = bossObjects[0].AnimationType
        };

        SpawnedEnemies = new Enemy[objs.Length];
        for (int i = 0; i < objs.Length; i++)
            SpawnedEnemies[i] = objs[i].GetComponent<Enemy>();

        NetworkServer.SendToAll(bossSpawnMessage);
        List<Player> players = PlayersDict.Instance.Players;
        for (int i = 0; i < players.Count; i++)
            players[i].canMove = false;

        enterAnimation = BossEnterAnimation.AddAnimationType(gameObject, bossObjects[0].AnimationType);
        enterAnimationCoroutine = new ExtendedCoroutine(this, enterAnimation.PlayAnimation(objs[0], this),
            StartCheckingForAllPlayersWatchedEnterAnimation, true);
    }

    public void StartBossAnimation(BossSpawnMessage bossSpawnMessage)
    {
        if (!Player.LocalPlayer || Player.LocalPlayer.isServer)
            return;

        BossEnterAnimation bea = BossEnterAnimation.AddAnimationType(gameObject, bossSpawnMessage.animationType);
        enterAnimationCoroutine = new ExtendedCoroutine(this, bea.PlayAnimation(bossSpawnMessage.bossGameObjects[0], this), Player.LocalPlayer.StateCommunicator.CmdBossAnimationFinished, true);
    }

    [Server]
    private void StartCheckingForAllPlayersWatchedEnterAnimation()
    {
        enterAnimationCoroutine = new ExtendedCoroutine(this, CheckForPlayersWatchedEnterAnimation(), StartBossEncounter, true);
    }

    private void StartBossEncounter()
    {
        List<Player> players = PlayersDict.Instance.Players;
        for (int i = 0; i < players.Count; i++)
        {
            players[i].canMove = true;
            players[i].StateCommunicator.bossAnimationFinished = false;
        }

        for (int i = 0; i < SpawnedEnemies.Length; i++)
            SpawnedEnemies[i].Brain.enabled = true;

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
        // Spawn exit to next level
        AlreadyCleared = true;

        GameManager.OnRoomEventEnded();
    }

    private GameObject[] SpawnBoss()
    {
        // TODO
        return null;
    }

    private IEnumerator CheckForPlayersWatchedEnterAnimation()
    {
        List<Player> players = PlayersDict.Instance.Players;
        float timer = 5.0f; // max time players have before game auto starts the encounter.
        while (true)
        {
            timer -= Time.deltaTime;

            bool everyoneReady = true;
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].isServer == true || players[i].StateCommunicator.bossAnimationFinished == false)
                {
                    everyoneReady = false;
                    break;
                }
            }

            if (everyoneReady == true || timer < 0.0f)
                yield break;

            yield return null;
        }
    }
}
