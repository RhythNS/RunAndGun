using MapGenerator;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dungeon room that cointains a boss and a zone to the next level.
/// </summary>
public class BossRoom : DungeonRoom
{
    public override bool EventOnRoomEntered => false;
    public override RoomType RoomType => RoomType.Boss;

    public State CurrentState { get; private set; } = State.ReadyToEnter;
    /// <summary>
    /// All spawned Bosses.
    /// </summary>
    public Enemy[] SpawnedEnemies { get; private set; }
    public bool IsLocked => CurrentState == State.Locked;
    /// <summary>
    /// The position that all players are teleported to when the boss fight starts.
    /// </summary>
    public Vector2 PlayersStartPos { get; private set; }

    /// <summary>
    /// The bosses that spawn inside this room.
    /// </summary>
    public BossObject[] bossObjects;

    private ExtendedCoroutine enterAnimationCoroutine;
    private BossEnterAnimation enterAnimation;

    /// <summary>
    /// States in which the Bossroom can be in.
    /// </summary>
    public enum State
    {
        Locked, ReadyToEnter, InProgress, Cleared, Failed
    }

    /// <summary>
    /// Set wheter the room is lock or not. Does not work if the room is already cleared or players
    /// are already fighting against the boss.
    /// </summary>
    /// <param name="locked">Wheter it is locked or not.</param>
    public void SetLocked(bool locked)
    {
        if (CurrentState == State.Locked || CurrentState == State.ReadyToEnter)
            CurrentState = locked ? State.Locked : State.ReadyToEnter;
    }

    public override void OnFullyCreated()
    {
        BossReadyZone readyPrefab = DungeonRoomObjectDict.Instance.BossReadyZone;
        Vector2 mid = Border.position + (Border.size * 0.5f);

        for (int i = 0; i < doors.Count; i++)
        {
            doors[i].IsLocked = true;
            BoxCollider2D coll = doors[i].GetComponent<BoxCollider2D>();

            Vector2 pos = doors[i].transform.position;
            Vector2 dir = pos - mid;
            Vector2 offset = doors[i].IsLeftRight ?
                (dir.x > 0 ? new Vector2(1, 0) : new Vector2(-1, 0)) // is left/right. Is it to the right or to the left?
                : (dir.y > 0 ? new Vector2(0, 1) : new Vector2(0, -1)); // is up/down. Is it to the up or the down?

            Rect rect = new Rect(pos + coll.offset + offset, coll.size * doors[i].transform.lossyScale);
            BossReadyZone brz = Instantiate(readyPrefab, transform);
            brz.Set(rect, doors[i]);
        }
    }

    /// <summary>
    /// Called when all players stood on a BoseReadyZone.
    /// </summary>
    /// <param name="bossReadyZone">The zone that the players stood on.</param>
    [Server]
    public void OnAllPlayersReadyToEnter(BossReadyZone bossReadyZone)
    {
        CurrentState = State.InProgress;

        GameObject[] objs = SpawnBoss();
        CloseDoors();

        PlayersStartPos = (bossReadyZone.OnDoor.transform.position - bossReadyZone.transform.position) * 2 + bossReadyZone.transform.position;

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
        
        // Make all players not being able to move.
        List<Player> players = PlayersDict.Instance.Players;
        for (int i = 0; i < players.Count; i++)
            players[i].RpcChangeCanMove(false);

        // Start the boss animations.
        enterAnimation = BossEnterAnimation.AddAnimationType(gameObject, bossObjects[0].AnimationType);
        enterAnimationCoroutine = new ExtendedCoroutine(this, enterAnimation.PlayAnimation(objs[0], this),
            StartCheckingForAllPlayersWatchedEnterAnimation, true);

        GameManager.OnRoomEventStarted();
    }

    /// <summary>
    /// Starts the boss animation for the local player.
    /// </summary>
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

    /// <summary>
    /// Starts the boss fight.
    /// </summary>
    private void StartBossEncounter()
    {
        // Make all players able to move again.
        List<Player> players = PlayersDict.Instance.Players;
        for (int i = 0; i < players.Count; i++)
        {
            players[i].RpcChangeCanMove(true);
            players[i].StateCommunicator.bossAnimationFinished = false;
        }

        // Make bosses able to move
        for (int i = 0; i < SpawnedEnemies.Length; i++)
            SpawnedEnemies[i].Brain.enabled = true;

        // Register for all enemies or players dead evetns.
        AliveHealthDict.Instance.OnAllEnemiesDied += OnAllEnemiesDefeated;
        AliveHealthDict.Instance.OnAllPlayersDied += OnAllPlayersDied;
    }

    /// <summary>
    /// Instantiates each boss and returns them.
    /// </summary>
    private GameObject[] SpawnBoss()
    {
        GameObject[] bosses = new GameObject[bossObjects.Length];
        for (int i = 0; i < bossObjects.Length; i++)
        {
            bosses[i] = Enemy.InstantiateAndSpawn(bossObjects[i], Border, Border.position + (Border.size * 0.5f), Quaternion.identity).gameObject;
        }

        return bosses;
    }

    /// <summary>
    /// Runs as long as either all players watched the boss enter animations or a timeout time has been
    /// reached.
    /// </summary>
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
                if (players[i].isServer == false && players[i].StateCommunicator.bossAnimationFinished == false)
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

    /// <summary>
    /// Callback if all players died.
    /// </summary>
    private void OnAllPlayersDied()
    {
        CurrentState = State.Failed;

        AliveHealthDict.Instance.OnAllEnemiesDied -= OnAllEnemiesDefeated;
        AliveHealthDict.Instance.OnAllPlayersDied -= OnAllPlayersDied;
    }

    /// <summary>
    /// Callback if all enemies are defeated.
    /// </summary>
    private void OnAllEnemiesDefeated()
    {
        CurrentState = State.Cleared;

        AliveHealthDict.Instance.OnAllEnemiesDied -= OnAllEnemiesDefeated;
        AliveHealthDict.Instance.OnAllPlayersDied -= OnAllPlayersDied;

        OpenDoors();
        // SpawnLoot();
        SpawnExitToNextLevel();
        AlreadyCleared = true;

        GameManager.OnRoomEventEnded();
    }

    /// <summary>
    /// Spawns the exit to the next level.
    /// </summary>
    [Server]
    private void SpawnExitToNextLevel()
    {
        AdvanceFloorZone afz = Instantiate(DungeonRoomObjectDict.Instance.AdvanceFloorZone, transform);
        afz.transform.position = Border.position + Border.size * 0.5f;
        NetworkServer.Spawn(afz.gameObject);
    }

    public override void OnLocalPlayerEntered()
    {
        if (CurrentState == State.Cleared)
            base.OnLocalPlayerEntered();
    }

    public override void OnLocalPlayerLeft()
    {
        if (CurrentState == State.Cleared)
            base.OnLocalPlayerLeft();
    }
}
