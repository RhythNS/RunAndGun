using Mirror;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the gameflow. This should only be active on the server.
/// </summary>
public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    /// <summary>
    /// All states that the GameManager can be in.
    /// </summary>
    public enum State
    {
        LoadingLevel, Wandering, RoomEvent, Cleared, Failed
    }

    /// <summary>
    /// The current state of the GameManager.
    /// </summary>
    public State CurrentState
    {
        get => currentState; private set
        {
            currentState = value;
            Debug.Log("Switch state to " + currentState);
        }
    }
    private State currentState = State.LoadingLevel;

    public static GameMode gameMode;
    public static int currentLevel;

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
        if (instance != this)
            return;

        PlayersDict.Instance.OnPlayerDisconnected += OnPlayerDisconnected;
        AliveHealthDict.Instance.OnAllPlayersDied += OnAllPlayersDied;
        currentLevel = -1;
    }

    /// <summary>
    /// Called when a player disconnected.
    /// </summary>
    /// <param name="player">The player that disconnected.</param>
    private void OnPlayerDisconnected(Player player)
    {
        Debug.Log(player.entityName + " has disconnected!");
        // TODO: Figure out what happens here.
        // If level is loading =>
        // If level is not loading => 
        // Trigger onplayerchangedroom
    }

    /// <summary>
    /// Called when a new game was started.
    /// </summary>
    /// <param name="gameMode">The gamemode that was selected.</param>
    /// <param name="seed">The seed of the game.</param>
    public static void OnStartNewGame(GameMode gameMode, int seed)
    {
        if (!instance)
            return;

        // Create or reset the Stats.
        if (instance.TryGetComponent(out StatTracker tracker) == false)
            instance.gameObject.AddComponent<StatTracker>();
        else
            tracker.ResetStats();

        GameMode copied = Instantiate(gameMode);
        copied.Init(seed);
        GameManager.gameMode = copied;
    }

    /// <summary>
    /// Called when a new Game started. Creates a random seed.
    /// </summary>
    /// <param name="gameMode">The sgamemode that was selected.</param>
    public static void OnStartNewGame(GameMode gameMode)
    {
        OnStartNewGame(gameMode, Random.Range(int.MinValue, int.MaxValue));
    }

    /// <summary>
    /// Called when a new level should be loaded.
    /// </summary>
    public static void OnLoadNewLevel()
    {
        if (!instance)
            return;

        ClearAllObjects();

        instance.CurrentState = State.LoadingLevel;
        ++currentLevel;
        if (currentLevel >= gameMode.levelAmount + 1)
        {
            OnDungeonCleared();
            return;
        }

        GenerateLevelMessage generateLevelMessage = new GenerateLevelMessage()
        {
            levelNumber = currentLevel,
            region = GetNextRegion()
        };

        NetworkServer.SendToAll(generateLevelMessage);
        List<Player> players = PlayersDict.Instance.Players;
        for (int i = 0; i < players.Count; i++)
        {
            players[i].RpcChangeCanMove(false);
        }
    }

    /// <summary>
    /// Gets the next region of the gamemode.
    /// </summary>
    private static Region GetNextRegion()
    {
        // Todo: delete this code and uncomment the next lines to renable the gamemode.
        // Thas was made to show different regions on the finished assigment.

        Region[] regionsToPickFrom = { Region.Castle, Region.Atlantis, Region.Dungeon };
        return RandomUtil.Element(regionsToPickFrom);

        /*
        if (gameMode.customRegionOnLevels == null || gameMode.customRegionOnLevels.Length == 0)
            return Region.Debug; // <--- TODO:

        for (int i = 1; i < gameMode.customRegionOnLevels.Length; i++)
        {
            if (gameMode.customRegionOnLevels[i].level > currentLevel)
                return gameMode.customRegionOnLevels[i - 1].region;
        }

        return gameMode.customRegionOnLevels[gameMode.customRegionOnLevels.Length - 1].region;
         */
    }

    /// <summary>
    /// Called when a player loaded the level and is waiting for the game to start.
    /// </summary>
    public static void OnPlayerLoadedLevelChanged()
    {
        if (!instance)
            return;

        if (instance.CurrentState != State.LoadingLevel)
        {
            Debug.LogWarning("Someone reported level change while playing!");
            return;
        }

        // Have all players finished loading?
        List<Player> players = PlayersDict.Instance.Players;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].StateCommunicator.levelLoaded == false)
                return;
        }

        OnLevelLoaded();
    }

    /// <summary>
    /// Called when the level finished loading.
    /// </summary>
    public static void OnLevelLoaded()
    {
        if (!instance)
            return;

        instance.CurrentState = State.Wandering;

        // Teleport all players into the start room.
        Vector3 toTeleport = new Vector3();
        DungeonRoom[] rooms = DungeonDict.Instance.Rooms;
        for (int i = 0; i < rooms.Length; i++)
        {
            if (rooms[i].RoomType == MapGenerator.RoomType.Start)
            {
                toTeleport = rooms[i].Border.position + (rooms[i].Border.size * 0.5f);
                break;
            }
        }
        List<Player> players = PlayersDict.Instance.Players;
        for (int i = 0; i < players.Count; i++)
        {
            players[i].StateCommunicator.levelLoaded = false;
            players[i].SmoothSync.teleportAnyObjectFromServer(toTeleport, Quaternion.identity, new Vector3(1, 1, 1));
            players[i].RpcChangeCanMove(true);
        }

        NetworkServer.SendToAll(new EveryoneLoadedMessage());

        DungeonDict.Instance.dungeon = DungeonCreator.Instance.dungeon; // TODO: Dungeon creator should set that itself
    }

    /// <summary>
    /// Called when all levels of the dungeon were cleared.
    /// </summary>
    public static void OnDungeonCleared()
    {
        if (!instance)
            return;

        instance.CurrentState = State.Cleared;
        // display dialog to go back to lobby
    }

    /// <summary>
    /// Called when a room event started.
    /// </summary>
    public static void OnRoomEventStarted()
    {
        if (!instance)
            return;

        instance.CurrentState = State.RoomEvent;
    }

    /// <summary>
    /// Called when a room event ended.
    /// </summary>
    public static void OnRoomEventEnded()
    {
        if (!instance)
            return;

        instance.CurrentState = State.Wandering;
    }

    /// <summary>
    /// Called when all players died.
    /// </summary>
    public void OnAllPlayersDied()
    {
        if (!instance)
            return;

        instance.CurrentState = State.Failed;
        GameOverMessage msg = new GameOverMessage(new StatsTransmission(StatTracker.Instance.GetAllStats()));
        NetworkServer.SendToAll(msg);
    }

    /// <summary>
    /// Returns all players back to the lobby.
    /// </summary>
    public static void BackToLobby()
    {
        if (!instance)
            return;

        ClearAllObjects();
        Destroy(instance);
        instance = null;
        NetworkServer.SendToAll(new ReturnToLobbyMessage());
    }

    /// <summary>
    /// Clears all kinds of objects from the world.
    /// </summary>
    public static void ClearAllObjects()
    {
        if (!instance)
            return;

        PickableInWorld[] pickableInWorlds = FindObjectsOfType<PickableInWorld>();
        for (int i = 0; i < pickableInWorlds.Length; i++)
            NetworkServer.Destroy(pickableInWorlds[i].gameObject);
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        for (int i = 0; i < enemies.Length; i++)
            NetworkServer.Destroy(enemies[i].gameObject);
    }

    /// <summary>
    /// Called when a player entered another room.
    /// </summary>
    public static void OnPlayerChangedRoom(Player player)
    {
        if (!instance)
            return;

        MiniMapManager.Instance.OnRoomEntered(player.CurrentRoom);

        if (instance.currentState != State.Wandering)
            return;

        List<Player> players = PlayersDict.Instance.Players;
        if (players.Count == 0)
            return;

        int firstAlivePlayer = -1;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].Health.Alive)
            {
                firstAlivePlayer = i;
                break;
            }
        }

        if (firstAlivePlayer == -1)
            return;

        DungeonRoom room = players[firstAlivePlayer].CurrentRoom;

        if (!room || !room.EventOnRoomEntered || room.AlreadyCleared)
            return;

        for (int i = firstAlivePlayer + 1; i < players.Count; i++)
        {
            if (!players[i].Health.Alive || players[i].CurrentRoom != room)
                return;
        }

        room.OnAllPlayersEntered();
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
