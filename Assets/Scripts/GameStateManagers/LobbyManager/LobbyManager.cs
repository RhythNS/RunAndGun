using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    private static LobbyManager instance;

    private List<LobbyPlayer> waitingPlayers = new List<LobbyPlayer>();

    private ExtendedCoroutine gameStartCountdown;

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

    public static void Register(LobbyPlayer lobbyPlayer)
    {
        instance.waitingPlayers.Add(lobbyPlayer);
    }

    public static void DeRegister(LobbyPlayer lobbyPlayer)
    {
        if (!instance)
            return;

        instance.waitingPlayers.Remove(lobbyPlayer);
        OnPlayerChangedReady();
    }

    public static void OnPlayerChangedReady()
    {
        // Player set unready during countdown?
        if (instance.gameStartCountdown != null)
        {
            for (int i = 0; i < instance.waitingPlayers.Count; i++)
            {
                if (!instance.waitingPlayers[i].isReady)
                {
                    instance.gameStartCountdown.Stop(false);
                    instance.gameStartCountdown = null;
                    return;
                }
            }
        }
        // All players are ready?
        else
        {
            for (int i = 0; i < instance.waitingPlayers.Count; i++)
            {
                if (!instance.waitingPlayers[i].isReady)
                {
                    return;
                }
            }
            instance.gameStartCountdown = new ExtendedCoroutine(instance, instance.StartGameCountdown(), instance.OnCountDownFinished, true);

        }
    }

    private void OnCountDownFinished()
    {
        instance = null;
        for (int i = 0; i < waitingPlayers.Count; i++)
        {
            Destroy(waitingPlayers[i]);
        }
        gameObject.AddComponent<GameManager>();
        Destroy(this);
    }

    private IEnumerator StartGameCountdown()
    {
        // notify ui
        yield return new WaitForSeconds(3.0f);
        // do other stuff
    }
}
