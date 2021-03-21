using System.Collections.Generic;
using UnityEngine;

public class PlayersDict : MonoBehaviour
{
    public delegate void PlayerDisconnected(Player player);
    public delegate void PlayerConnected(Player player);

    public static PlayersDict Instance { get; private set; }

    public List<Player> Players { get; private set; } = new List<Player>();
    public event PlayerDisconnected OnPlayerDisconnected;
    public event PlayerConnected OnPlayerConnected;

    public Color[] PlayerColors => playerColors;
    [SerializeField] private Color[] playerColors;

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

    public void Register(Player player)
    {
        Players.Add(player);
        OnPlayerConnected?.Invoke(player);
    }

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
