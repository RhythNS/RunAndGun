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

    public void HostMatch(Dictionary<string, MatchData> matchData)
    {
        NetworkManager.singleton.StartHost();

        /*
        var matchData = new Dictionary<string, MatchData>() {
                { "Match name", "Layla's Match" },
                { "eloScore", 200 },
                { "Region", "North America" },
                { "Map name", "de_dust" },
                { "Game type", "Capture the flag" },
            };
         */

        matchUp.CreateMatch(NetworkManager.singleton.maxConnections + 1, matchData, OnMatchCreated);
    }

    public void OnMatchCreated(bool success, Match match)
    {
        if (success)
        {
            Debug.Log("Created match: " + match.matchData["Match name"]);
        }
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

    /*
    // Called when the match list is retreived via GetMatchList
    private void OnMatchListReceived(bool success, Match[] matches)
    {
        if (!success) return;

        Debug.Log("Received match list.");
        this.matches = matches;
    }
     */

    public void JoinMatch(Match match, Action<bool, Match> onJoinMatch)
    {
        if (match == null)
            return;

        matchUp.JoinMatch(match, onJoinMatch);
    }

    /*
    // Called when a response is received from a JoinMatch request
    private void OnJoinMatch(bool success, Match match)
    {
        if (!success) return;

        // We don't need to keep the list around any more
        matches = null;

        Debug.Log("Joined match: " + match.matchData["Match name"]);

        NetworkManager.singleton.StartClient(match);
    }
     */

    public void SetMatchData()
    {
        Debug.Log("Setting match data");

        /**
         * Option 2: Completely replace existing match data and immediately send it to the matchmaking server
         */
        //var newMatchData = new Dictionary<string, MatchData>() {
        //    { "Key1", "value1" },
        //    { "Key2", 3.14159 }
        //};
        //matchUp.SetMatchData(newMatchData);
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
