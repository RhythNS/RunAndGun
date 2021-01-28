using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public enum State
    {
        LoadingLevel, Wandering, RoomEvent, Cleared, Failed
    }

    public State CurrentState
    {
        get => currentState; private set
        {
            currentState = value;
            Debug.Log("Switch state to " + currentState);
        }
    }
    private State currentState = State.LoadingLevel;

    [SerializeField] private int checkRoomsPerFrame = 3;

    private Coroutine checkForRoomEntered;

    private void Awake()
    {
        if (instance)
        {
            Debug.LogWarning("Already a GameManager in the scene. Deleting myself!");
            Destroy(this);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        if (instance == this)
        {
            PlayersDict.Instance.OnPlayerDisconnected += OnPlayerDisconnected;
            AliveHealthDict.Instance.OnAllPlayersDied += OnAllPlayersDied;
        }
    }

    private void OnPlayerDisconnected(Player player)
    {
        // TODO: Figure out what happens here.
        // If level is loading =>
        // If level is not loading => 
    }

    public static void OnLoadNewLevel()
    {
        if (!instance)
            return;

        instance.CurrentState = State.LoadingLevel;
    }

    public static void OnPlayerLoadedLevelChanged()
    {
        if (!instance)
            return;

        if (instance.CurrentState != State.LoadingLevel)
        {
            Debug.LogWarning("Someone reported level change while playing!");
            return;
        }

        List<Player> players = PlayersDict.Instance.Players;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].StateCommunicator.levelLoaded == false)
                return;
        }

        OnLevelLoaded();
    }


    public static void OnLevelLoaded()
    {
        if (!instance)
            return;

        instance.CurrentState = State.Wandering;

        List<Player> players = PlayersDict.Instance.Players;
        for (int i = 0; i < players.Count; i++)
        {
            players[i].StateCommunicator.levelLoaded = false;
        }

        DungeonDict.Instance.dungeon = DungeonCreator.Instance.dungeon; // TODO: Dungeon creator should set that itself
        instance.checkForRoomEntered = instance.StartCoroutine(instance.CheckForRoomEnter());
    }

    public static void OnLevelCleared()
    {
        if (!instance)
            return;

        DungeonDict.Instance.ClearRooms();
        instance.StopAllCoroutines();

        instance.CurrentState = State.Cleared;
        // display dialog to load next level
    }

    public static void OnRoomEventStarted(Rect bounds)
    {
        if (!instance)
            return;

        instance.CurrentState = State.RoomEvent;

        instance.StopCoroutine(instance.checkForRoomEntered);
        // ui.HideAroundBounds(bounds);
    }

    public static void OnRoomEventEnded()
    {
        if (!instance)
            return;

        instance.CurrentState = State.Wandering;

        instance.checkForRoomEntered = instance.StartCoroutine(instance.CheckForRoomEnter());
        // ui.StopHidingAroundBounds();
    }

    public void OnAllPlayersDied()
    {
        instance.StopAllCoroutines();

        instance.CurrentState = State.Failed;

        // for all clients -> send game over
    }

    private void OnBackToLobby()
    {
        gameObject.AddComponent<LobbyManager>();
        Destroy(this);
        NetworkServer.SendToAll(new ReturnToLobbyMessage());
    }


    private IEnumerator CheckForRoomEnter()
    {
        while (true)
        {
            int counter = 0;

            List<Health> playerHealths = AliveHealthDict.Instance.PlayerHealths;

            if (playerHealths.Count == 0)
                yield break;

            List<Bounds> playerBounds = new List<Bounds>(playerHealths.Count);

            for (int i = 0; i < playerHealths.Count; i++)
                playerBounds.Add(playerHealths[i].GetComponent<Player>().Collider2D.bounds);

            DungeonRoom[] rooms = DungeonDict.Instance.Rooms;

            for (int i = 0; i < rooms.Length; i++)
            {
                // if room has a local player event
                //if (rooms[i].CheckLocalPlayerEntered(Player.LocalPlayer.Collider2D.bounds))
                //    rooms[i].OnLocalPlayerEntered();

                // If the room has no event or has an event but is already cleared, skip this room.
                if (!rooms[i].EventOnRoomEntered || rooms[i].AlreadyCleared)
                    continue;

                ++counter;

                if (rooms[i].CheckAllPlayersEntered(playerBounds))
                {
                    rooms[i].OnAllPlayersEntered();
                    yield break;
                }

                // Already checked enough rooms?
                if (counter >= checkRoomsPerFrame)
                    yield return null;
            }

            yield return null;
        }
    }


    private void OnDestroy()
    {
        if (instance == this)
        {
            if (AliveHealthDict.Instance)
                AliveHealthDict.Instance.OnAllPlayersDied -= OnAllPlayersDied;
            if (PlayersDict.Instance)
                PlayersDict.Instance.OnPlayerDisconnected -= OnPlayerDisconnected;
            instance = null;
        }
    }
}
