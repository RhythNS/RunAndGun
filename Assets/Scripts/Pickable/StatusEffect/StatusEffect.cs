using UnityEngine;

public abstract class StatusEffect : Pickable
{
    public override PickableType PickableType => PickableType.StatusEffect;

    [SerializeField] protected int numberOfTicks;

    public StatusEffectList OnList { get; set; }
    public Health OnHealth => OnList.Health;
    public Health Inflicter { get; set; } = null;
    public bool IsInstant => numberOfTicks == 0;
    public bool IsForever => numberOfTicks == -1;
    public bool IsServer => OnList.isServer;
    public bool IsLocalPlayer => OnList.isLocalPlayer;
    public int NumberOfTicksRemaining { get; protected set; }
    public int NumberOfTicks => numberOfTicks;

    public void OnPickup()
    {
        NumberOfTicksRemaining = numberOfTicks;
        InnerOnPickup();
    }

    protected abstract void InnerOnPickup();

    public abstract void OnDrop();

    /// <summary>
    /// Instance of this is in list. The other StatusEffect tried to add itself to the list.
    /// </summary>
    public abstract void OnEffectAlreadyInList(StatusEffect other);

    public void OnTick()
    {
        InnerOnTick();

        if (!OnList.isServer || numberOfTicks == -1)
            return;

        if (--NumberOfTicksRemaining <= 0)
            OnList.ServerRemove(this);
    }

    protected abstract void InnerOnTick();
}
