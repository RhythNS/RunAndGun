using Mirror;
using UnityEngine;

/// <summary>
/// Holds all stats and formulas that need these stats.
/// </summary>
public class Stats : NetworkBehaviour
{
    // TODO: Remove SerializeField
    // TODO: Add hooks to UI elements when variables are changing 
    [SyncVar] [SerializeField] private int baseHealth;
    [SyncVar] [SerializeField] private int health;
    [SyncVar] [SerializeField] private int baseDefence;
    [SyncVar] [SerializeField] private int defence;
    [SyncVar] [SerializeField] private int baseSpeed;
    [SyncVar] [SerializeField] private int speed;
    [SyncVar] [SerializeField] private int baseLuck;
    [SyncVar] [SerializeField] private int luck;
    [SyncVar] [SerializeField] private int baseDodge;
    [SyncVar] [SerializeField] private int dodge;

    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
        health = baseHealth;
        defence = baseDefence;
        speed = baseSpeed;
        luck = baseLuck;
        dodge = baseDodge;
    }

    /// <summary>
    /// Called when the item list changed.
    /// </summary>
    /// <param name="items">A list of current equipped items.</param>
    [Server]
    public void OnItemsChanged(SyncList<Item> items)
    {
        StatsEffect effect = new StatsEffect(baseHealth, baseDefence, baseSpeed, baseLuck, baseDodge);
        for (int i = 0; i < items.Count; i++)
            items[i].OnHold(player, ref effect);
        SetStats(effect);
    }

    /// <summary>
    /// Normailzes each stat.
    /// </summary>
    /// <param name="effect">The stats to be normalized</param>
    private void NormalizeStats(ref StatsEffect effect)
    {
        effect.health = Mathf.Clamp(effect.health, 1, 10);
        effect.defence = Mathf.Clamp(effect.defence, 1, 10);
        effect.speed = Mathf.Clamp(effect.speed, 1, 10);
        effect.luck = Mathf.Clamp(effect.luck, 1, 10);
        effect.dodge = Mathf.Clamp(effect.dodge, 1, 10);
    }

    /// <summary>
    /// Sets all stats.
    /// </summary>
    [Server]
    private void SetStats(StatsEffect effect)
    {
        NormalizeStats(ref effect);

        if (effect.health != health)
        {
            health = effect.health;
            player.Health.SetMax(health);
        }
        if (effect.defence != defence)
        {
            defence = effect.defence;
            player.Health.SetDefence(defence);
        }
        if (effect.speed != speed)
        {
            speed = effect.speed;
            player.Input.movementForce = GetMovementForce();
        }
        if (effect.luck != luck)
        {
            luck = effect.luck;

        }
        if (effect.dodge != dodge)
        {
            dodge = effect.dodge;
            player.Status.SetDashCooldown(GetDodgeCooldown());
        }
    }

    /// <summary>
    /// Gets the current maximum movment force.
    /// </summary>
    public float GetMovementForce()
    {
        // TODO: Some fitting formula
        return speed * 600;
    }

    /// <summary>
    /// Gets the dodge cooldown in seconds.
    /// </summary>
    public float GetDodgeCooldown()
    {
        // TODO: Some fitting formula
        return 3.0f * (1 - (float)(Mathf.Max(1, 10 - dodge) / 10));
    }
}
