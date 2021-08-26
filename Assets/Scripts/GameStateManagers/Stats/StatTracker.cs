using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager for tracking stats for all players.
/// </summary>
public class StatTracker : MonoBehaviour
{
    public static StatTracker Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("StatTracker already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Get a dictionary of all stats for a player.
    /// </summary>
    private Dictionary<Player, Dictionary<Type, Stat>> statsForPlayer = new Dictionary<Player, Dictionary<Type, Stat>>();

    /// <summary>
    /// Gets a Stat for a specfic Player. Creates a stat if no stat of the specifiec kind was found.
    /// </summary>
    /// <typeparam name="T">The type of the Stat.</typeparam>
    /// <param name="player">The player.</param>
    /// <returns>A reference to the stat.</returns>
    public T GetStat<T>(Player player) where T : Stat, new()
    {
        Dictionary<Type, Stat> stats = GetStats(player);

        if (stats.TryGetValue(typeof(T), out Stat stat) == true)
            return (T)stat;

        T t = new T();
        stats.Add(typeof(T), t);
        return t;
    }

    /// <summary>
    /// Gets all stats for a player.
    /// </summary>
    /// <param name="player">The player of which to get the stats from.</param>
    public Dictionary<Type, Stat> GetStats(Player player)
    {
        if (statsForPlayer.TryGetValue(player, out Dictionary<Type, Stat> dict) == true)
            return dict;

        dict = new Dictionary<Type, Stat>();
        statsForPlayer.Add(player, dict);
        return dict;
    }

    /// <summary>
    /// Gets all stats for all players.
    /// </summary>
    public Dictionary<Player, Dictionary<Type, Stat>> GetAllStats()
    {
        return statsForPlayer;
    }

    /// <summary>
    /// Resets all stats.
    /// </summary>
    public void ResetStats()
    {
        statsForPlayer = new Dictionary<Player, Dictionary<Type, Stat>>();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
