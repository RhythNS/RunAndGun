using System;
using System.Collections.Generic;
using UnityEngine;

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

    private Dictionary<Player, Dictionary<Type, Stat>> statsForPlayer = new Dictionary<Player, Dictionary<Type, Stat>>();

    public T GetStat<T>(Player player) where T : Stat, new()
    {
        Dictionary<Type, Stat> stats = GetStats(player);

        if (stats.TryGetValue(typeof(T), out Stat stat) == true)
            return (T)stat;

        T t = new T();
        stats.Add(typeof(T), t);
        return t;
    }

    public Dictionary<Type, Stat> GetStats(Player player)
    {
        if (statsForPlayer.TryGetValue(player, out Dictionary<Type, Stat> dict) == true)
            return dict;

        dict = new Dictionary<Type, Stat>();
        statsForPlayer.Add(player, dict);
        return dict;
    }

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
