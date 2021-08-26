using MatchUp;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the communication with the list server and matchmaking.
/// </summary>
[RequireComponent(typeof(Matchmaker))]
public class RAGMatchmaker : MonoBehaviour
{
    public static RAGMatchmaker Instance { get; private set; }
    public bool IsReady => matchUp.IsReady;

    private Matchmaker matchUp;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("RAGMatchmaker already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;

        matchUp = GetComponent<Matchmaker>();
        matchUp.onLostConnectionToMatchmakingServer = OnLostConnectionToMatchmakingServer;
    }

    /// <summary>
    /// Gets the current match that the client is either hosting or is connected to. Can be null.
    /// </summary>
    public Match GetCurrentMatch() => matchUp.currentMatch;

    /// <summary>
    /// Tries to reconnect to the matchmaker service.
    /// </summary>
    public IEnumerator Reconnect() => matchUp.ConnectToMatchmaker();

    /// <summary>
    /// Hosts a match.
    /// </summary>
    /// <param name="matchData">The matchdata describing the match.</param>
    /// <param name="maxPlayers">The max players that can connect to the match.</param>
    /// <param name="onMatchCreated">Callback when the match was created.</param>
    public void HostMatch(Dictionary<string, MatchData> matchData, int maxPlayers, Action<bool, Match> onMatchCreated)
    {
        NetworkManager.singleton.maxConnections = maxPlayers;
        // NetworkManager.singleton.StartHost();
        matchUp.CreateMatch(maxPlayers, matchData, onMatchCreated);
    }

    /// <summary>
    /// Get a list of all matches.
    /// </summary>
    /// <param name="onMatchListRecieved">Callback when the match list was recieved.</param>
    public void GetMatchList(Action<bool, Match[]> onMatchListRecieved)
    {
        matchUp.GetMatchList(onMatchListRecieved, 0, 40);
    }

    /// <summary>
    /// Joins a match.
    /// </summary>
    /// <param name="match">The match to be joined.</param>
    /// <param name="onJoinMatch">Callback when the match was joined.</param>
    public void JoinMatch(Match match, Action<bool, Match> onJoinMatch)
    {
        matchUp.JoinMatch(match, onJoinMatch);
    }

    /// <summary>
    /// Updates the match data of a hosted match.
    /// </summary>
    /// <param name="matchData">Callback when the matchdata was updated.</param>
    public void SetMatchData(Dictionary<string, MatchData> matchData)
    {
        matchUp.SetMatchData(matchData);
    }

    /// <summary>
    /// Disconnects from the matchup service.
    /// </summary>
    public void Disconnect()
    {
        if (NetworkServer.active)
            matchUp.DestroyMatch();
        else
            matchUp.LeaveMatch();
    }

    /// <summary>
    /// Callback when the connection to the matchmaking server was lost.
    /// </summary>
    private void OnLostConnectionToMatchmakingServer(Exception e)
    {
        Debug.LogError("Lost connection to matchmaking server: " + e);

        // TODO: Reconnect or handle this error.
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
