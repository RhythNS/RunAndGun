using MatchUp;
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Matchmaker))]
public class RAGMatchmaker : MonoBehaviour
{
    public static RAGMatchmaker Instance { get; private set; }

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

    public Match GetCurrentMatch()
    {
        return matchUp.currentMatch;
    }

    public void HostMatch(Dictionary<string, MatchData> matchData, Action<bool, Match> onMatchCreated)
    {
        NetworkManager.singleton.StartHost();
        matchUp.CreateMatch(NetworkManager.singleton.maxConnections + 1, matchData, onMatchCreated);
    }

    // Get a filtered list of matches
    public void GetMatchList(Action<bool, Match[]> onMatchListRecieved)
    {
        Debug.Log("Fetching match list");

        // Filter so that we only receive matches with 
        // an eloScore between 150 and 350
        // and the Region is North America
        // and the Game type is Capture the Flag
        var filters = new List<MatchFilter>(){
                new MatchFilter("eloScore", 150, MatchFilter.OperationType.GREATER),
                new MatchFilter("eloScore", 350, MatchFilter.OperationType.LESS),
                new MatchFilter("Region", "North America", MatchFilter.OperationType.EQUALS),
                new MatchFilter("Game type", "Capture the flag", MatchFilter.OperationType.EQUALS)
            };

        // Get the filtered match list. The results will be received in OnMatchListReceived()
        matchUp.GetMatchList(onMatchListRecieved, 0, 10, filters);
    }

    public void JoinMatch(Match match, Action<bool, Match> onJoinMatch)
    {
        if (match == null)
            return;

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
