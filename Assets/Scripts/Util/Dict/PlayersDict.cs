using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dict for information about current connected players.
/// </summary>
public class PlayersDict : MonoBehaviour
{
    public delegate void PlayerDisconnected(Player player);
    public delegate void PlayerConnected(Player player);

    public static PlayersDict Instance { get; private set; }

    public List<Player> Players { get; private set; } = new List<Player>();
    public event PlayerDisconnected OnPlayerDisconnected;
    public event PlayerConnected OnPlayerConnected;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("Already a PlayersDict in scene! Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Registers a new player to this dict.
    /// </summary>
    /// <param name="player">The player to be added.</param>
    public void Register(Player player)
    {
        Players.Add(player);
        OnPlayerConnected?.Invoke(player);
    }

    /// <summary>
    /// Deregisters a player from this dict.
    /// </summary>
    /// <param name="player">The player to be removed.</param>
    public void DeRegister(Player player)
    {
        Players.Remove(player);
        OnPlayerDisconnected?.Invoke(player);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
