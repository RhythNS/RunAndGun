using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameManager for handling the lobby region.
/// </summary>
public class LobbyManager : MonoBehaviour
{
    private static LobbyManager instance;

    private ExtendedCoroutine gameStartCountdown;

    [SerializeField] private GameMode selectedGameMode;

    private void Awake()
    {
        if (instance)
        {
            Debug.LogWarning("LobbyManager already in scene! Deleting myself!");
            Destroy(this);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        PlayersDict.Instance.OnPlayerDisconnected += OnPlayerChanged;
        PlayersDict.Instance.OnPlayerConnected += OnPlayerChanged;
        LobbyLevel.Instance.Show();
    }

    /// <summary>
    /// Called when the list of connected players changed.
    /// </summary>
    /// <param name="player">The player that was added or removed.</param>
    public void OnPlayerChanged(Player player)
    {
        OnPlayerChangedReady();
    }

    /// <summary>
    /// Changes the selected game mode.
    /// </summary>
    /// <param name="gameMode">The new gamemode.</param>
    public static void ChangeGameMode(GameMode gameMode)
    {
        instance.selectedGameMode = gameMode;
    }

    /// <summary>
    /// Called when a player changed its status to be ready for entering the main game.
    /// </summary>
    public static void OnPlayerChangedReady()
    {
        if (!instance)
            return;

        List<Player> players = PlayersDict.Instance.Players;

        if (players.Count == 0)
            return;

        // Player set unready during countdown?
        if (instance.gameStartCountdown != null)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (!players[i].StateCommunicator.lobbyReady)
                {
                    Debug.Log("Player unready! Stopping Countdown!");
                    instance.gameStartCountdown.Stop(false);
                    instance.gameStartCountdown = null;
                    return;
                }
            }
        }
        // All players are ready?
        else
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (!players[i].StateCommunicator.lobbyReady)
                {
                    return;
                }
            }
            instance.gameStartCountdown = new ExtendedCoroutine(instance, instance.StartGameCountdown(), instance.OnCountDownFinished, true);

        }
    }

    /// <summary>
    /// Called when the countdown for starting the game finished.
    /// </summary>
    private void OnCountDownFinished()
    {
        List<Player> players = PlayersDict.Instance.Players;
        for (int i = 0; i < players.Count; i++)
        {
            players[i].StateCommunicator.lobbyReady = false;
        }

        instance = null;
        GlobalsDict.Instance.GameStateManagerObject.AddComponent<GameManager>();

        int seed = selectedGameMode.randomSeed ? Random.Range(int.MinValue, int.MaxValue) : selectedGameMode.seed;

        GameManager.OnStartNewGame(selectedGameMode, seed);

        StartGameMessage sgm = new StartGameMessage
        {
            levelSeed = seed,
            gameMode = selectedGameMode
        };

        NetworkServer.SendToAll(sgm);

        GameManager.OnLoadNewLevel();

        Destroy(this);
    }

    /// <summary>
    /// Starts the countdown if all players are ready.
    /// </summary>
    private IEnumerator StartGameCountdown()
    {
        Debug.Log("All players ready! Starting Countdown!");
        // notify ui
        for (int i = 0; i > 0; i--)
        {
            Debug.Log(i + "!");
            yield return new WaitForSeconds(1.0f);
        }
        // disable Lobby
    }

    private void OnDestroy()
    {
        if (PlayersDict.Instance)
        {
            PlayersDict.Instance.OnPlayerDisconnected -= OnPlayerChanged;
            PlayersDict.Instance.OnPlayerConnected -= OnPlayerChanged;
        }
    }
}
