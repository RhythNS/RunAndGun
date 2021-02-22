using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    private static LobbyManager instance;

    private ExtendedCoroutine gameStartCountdown;

    [SerializeField] private GameMode testGame;

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
        DontDestroyOnLoad(gameObject);
        PlayersDict.Instance.OnPlayerDisconnected += OnPlayerChanged;
        PlayersDict.Instance.OnPlayerConnected += OnPlayerChanged;
        LobbyLevel.Instance.Show();
    }

    public void OnPlayerChanged(Player player)
    {
        OnPlayerChangedReady();
    }

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

    private void OnCountDownFinished()
    {
        List<Player> players = PlayersDict.Instance.Players;
        for (int i = 0; i < players.Count; i++)
        {
            players[i].StateCommunicator.lobbyReady = false;
        }

        instance = null;
        gameObject.AddComponent<GameManager>();
        GameManager.OnStartNewGame(testGame);


        // TODO: Maybe seed can be set in the menu?
        StartGameMessage sgm = new StartGameMessage
        {
            levelSeed = Random.Range(int.MinValue, int.MaxValue),
            gameMode = testGame
        };

        NetworkServer.SendToAll(sgm);

        GameManager.OnLoadNewLevel();

        Destroy(this);
    }

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
