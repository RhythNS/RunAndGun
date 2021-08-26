using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dict for all current alive healths in the scene.
/// </summary>
public class AliveHealthDict : MonoBehaviour
{
    public delegate void PlayerDied(Player player);
    public delegate void AllPlayersDied();
    public delegate void AllEnemiesDied();

    public static AliveHealthDict Instance { get; private set; }

    public event PlayerDied OnPlayerDied;
    public event AllPlayersDied OnAllPlayersDied;
    public event AllEnemiesDied OnAllEnemiesDied;

    public List<Health> PlayerHealths => playerHealths;
    [SerializeField] private List<Health> playerHealths;
    public List<Health> EnemyHealths => enemyHealths;
    [SerializeField] private List<Health> enemyHealths;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("Already a AliveHealthDict in scene! Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Registers a new health to the dict.
    /// </summary>
    /// <param name="health">The health to be registered.</param>
    public void Register(Health health)
    {
        if (health.TryGetComponent<Player>(out _))
            playerHealths.Add(health);
        else if (health.TryGetComponent<Enemy>(out _))
            enemyHealths.Add(health);
    }

    /// <summary>
    /// Deregisters a health from the dict.
    /// </summary>
    /// <param name="health">The health to be deregistered.</param>
    public void DeRegister(Health health)
    {
        if (health.TryGetComponent(out Player player))
        {
            playerHealths.Remove(health);
            OnPlayerDied?.Invoke(player);
            if (playerHealths.Count == 0)
                OnAllPlayersDied?.Invoke();
        }
        else if (health.TryGetComponent<Enemy>(out _))
        {
            enemyHealths.Remove(health);
            if (enemyHealths.Count == 0)
                OnAllEnemiesDied?.Invoke();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
