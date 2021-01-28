﻿using Mirror;
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
        // Trigger onplayerchangedroom
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
    }

    public static void OnLevelCleared()
    {
        if (!instance)
            return;

        DungeonDict.Instance.ClearRooms();

        instance.CurrentState = State.Cleared;
        // display dialog to load next level
    }

    public static void OnRoomEventStarted(Rect bounds)
    {
        if (!instance)
            return;

        instance.CurrentState = State.RoomEvent;

        // ui.HideAroundBounds(bounds);
    }

    public static void OnRoomEventEnded()
    {
        if (!instance)
            return;

        instance.CurrentState = State.Wandering;

        // ui.StopHidingAroundBounds();
    }

    public void OnAllPlayersDied()
    {
        instance.CurrentState = State.Failed;

        // for all clients -> send game over
    }

    private void OnBackToLobby()
    {
        gameObject.AddComponent<LobbyManager>();
        Destroy(this);
        NetworkServer.SendToAll(new ReturnToLobbyMessage());
    }

    public static void OnPlayerChangedRoom(Player player)
    {
        if (!instance || instance.currentState != State.Wandering)
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

        if (!room.EventOnRoomEntered || room.AlreadyCleared)
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
