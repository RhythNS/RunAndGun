using Mirror;
using UnityEngine;

/// <summary>
/// Holds all stats and formulas that need these stats.
/// </summary>
public class Stats : NetworkBehaviour
{
    // TODO: Remove SerializeField
    // TODO: Add hooks to UI elements when variables are changing
    public int BaseHealth => baseHealth;
    [SyncVar] [SerializeField] private int baseHealth;
    public int Health => health;
    [SyncVar] [SerializeField] private int health;
    public event IntChanged OnHealthChanged;

    public int BaseSpeed => baseSpeed;
    [SyncVar] [SerializeField] private int baseSpeed;
    public int Speed => speed;
    [SyncVar] [SerializeField] private int speed;
    public event IntChanged OnSpeedChanged;

    public int BaseLuck => baseLuck;
    [SyncVar] [SerializeField] private int baseLuck;
    public int Luck => luck;
    [SyncVar] [SerializeField] private int luck;
    public event IntChanged OnLuckChanged;

    public int BaseDodge => baseDodge;
    [SyncVar] [SerializeField] private int baseDodge;
    public int Dodge => dodge;
    [SyncVar] [SerializeField] private int dodge;
    public event IntChanged OnDodgeChanged;

    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
        health = baseHealth;
        speed = baseSpeed;
        luck = baseLuck;
        dodge = baseDodge;
    }

    public override void OnStartServer()
    {
        player.Health.SetMax(PlayerStatsDict.Instance.GetHealth(health));
    }

    /// <summary>
    /// Called when the item list changed.
    /// </summary>
    /// <param name="items">A list of current equipped items.</param>
    [Server]
    public void OnItemsChanged(SyncList<Item> items)
    {
        StatsEffect effect = new StatsEffect(baseHealth, baseSpeed, baseLuck, baseDodge);
        for (int i = 0; i < items.Count; i++)
            items[i].OnHold(player, ref effect);
        SetStats(effect);
    }

    /// <summary>
    /// Sets all stats.
    /// </summary>
    [Server]
    private void SetStats(StatsEffect effect)
    {
        PlayerStatsDict.Instance.NormalizeStats(ref effect);

        if (effect.health != health)
        {
            health = effect.health;
            player.Health.SetMax(PlayerStatsDict.Instance.GetHealth(health));
            OnHealthChanged?.Invoke(health);
        }
        if (effect.speed != speed)
        {
            speed = effect.speed;
            OnSpeedChanged?.Invoke(speed);
        }
        if (effect.luck != luck)
        {
            luck = effect.luck;
            OnLuckChanged?.Invoke(luck);
        }
        if (effect.dodge != dodge)
        {
            dodge = effect.dodge;
            OnDodgeChanged?.Invoke(dodge);
        }
    }
}
