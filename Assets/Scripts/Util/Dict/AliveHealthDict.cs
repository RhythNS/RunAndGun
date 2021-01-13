using System.Collections.Generic;
using UnityEngine;

public class AliveHealthDict : MonoBehaviour
{
    public static AliveHealthDict Instance { get; private set; }

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

    public void Register(Health health)
    {
        if (health.TryGetComponent<Player>(out _))
            playerHealths.Add(health);
        else if (health.TryGetComponent<Enemy>(out _))
            enemyHealths.Add(health);
    }

    public void DeRegister(Health health)
    {
        if (health.TryGetComponent<Player>(out _))
            playerHealths.Remove(health);
        else if (health.TryGetComponent<Enemy>(out _))
            enemyHealths.Remove(health);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
