using MatchUp;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Match GetCurrentMatch() => matchUp.currentMatch;

    public IEnumerator Reconnect() => matchUp.ConnectToMatchmaker();

    public void HostMatch(Dictionary<string, MatchData> matchData, int maxPlayers, Action<bool, Match> onMatchCreated)
    {
        NetworkManager.singleton.maxConnections = maxPlayers;
        NetworkManager.singleton.StartHost();
        matchUp.CreateMatch(maxPlayers, matchData, onMatchCreated);
    }

    public void GetMatchList(Action<bool, Match[]> onMatchListRecieved)
    {
        matchUp.GetMatchList(onMatchListRecieved, 0, 40);
    }

    public void JoinMatch(Match match, Action<bool, Match> onJoinMatch)
    {
        matchUp.JoinMatch(match, onJoinMatch);
    }

    public void SetMatchData(Dictionary<string, MatchData> matchData)
    {
        matchUp.SetMatchData(matchData);
    }

    public void Disconnect()
    {
        if (NetworkServer.active)
            matchUp.DestroyMatch();
        else
            matchUp.LeaveMatch();
    }

    private void OnLostConnectionToMatchmakingServer(Exception e)
    {
        Debug.LogError("Lost connection to matchmaking server: " + e);

        // Handle this however you feel is appropriate. 
        // The Exception will have the actual error but it was probably a loss of internet or server crash
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
