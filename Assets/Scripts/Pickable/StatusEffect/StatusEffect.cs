using UnityEngine;

/// <summary>
/// An effect that can be added to an entity with an StatusEffectList.
/// </summary>
public abstract class StatusEffect : Pickable
{
    public override PickableType PickableType => PickableType.StatusEffect;

    /// <summary>
    /// How many ticks the status effect is on for.
    /// </summary>
    [SerializeField] protected int numberOfTicks;

    /// <summary>
    /// A reference to the list that the status effect is on.
    /// </summary>
    public StatusEffectList OnList { get; set; }
    /// <summary>
    /// A reference to the health that the status effect is affecting.
    /// </summary>
    public Health OnHealth => OnList.Health;
    /// <summary>
    /// A reference to the health that was responsible for adding the status effect.
    /// Can be null.
    /// </summary>
    public Health Inflicter { get; set; } = null;
    /// <summary>
    /// Wheter the effect is instant and should not be added to a status effect list.
    /// </summary>
    public bool IsInstant => numberOfTicks == 0;
    /// <summary>
    /// Wheter the status effect is forever on the status effect list.
    /// </summary>
    public bool IsForever => numberOfTicks == -1;
    /// <summary>
    /// Wheter the status effect is on an entity that is managed by the server.
    /// </summary>
    public bool IsServer => OnList.isServer;
    /// <summary>
    /// Wheter the status effect is on an entity that is controlled by the loadl player.
    /// </summary>
    public bool IsLocalPlayer => OnList.isLocalPlayer;
    /// <summary>
    /// How many ticks are remaining for the status effect.
    /// </summary>
    public int NumberOfTicksRemaining { get; protected set; }
    /// <summary>
    /// The number of ticks that status effect is ticking for.
    /// </summary>
    public int NumberOfTicks => numberOfTicks;

    /// <summary>
    /// Called when the status effect was first picked up.
    /// </summary>
    public void OnPickup()
    {
        NumberOfTicksRemaining = numberOfTicks;
        InnerOnPickup();
    }

    /// <summary>
    /// Called when the status effect was first picked up.
    /// </summary>
    protected abstract void InnerOnPickup();

    /// <summary>
    /// Called when the status effect was removed.
    /// </summary>
    public abstract void OnDrop();

    /// <summary>
    /// Instance of this is in list. The other StatusEffect tried to add itself to the list.
    /// </summary>
    public abstract void OnEffectAlreadyInList(StatusEffect other);

    /// <summary>
    /// Called when the status effect ticked.
    /// </summary>
    public void OnTick()
    {
        InnerOnTick();

        if (!OnList.isServer || numberOfTicks == -1)
            return;

        if (--NumberOfTicksRemaining <= 0)
            OnList.ServerRemove(this);
    }

    /// <summary>
    /// Called when the status effect ticked.
    /// </summary>
    protected abstract void InnerOnTick();
}
