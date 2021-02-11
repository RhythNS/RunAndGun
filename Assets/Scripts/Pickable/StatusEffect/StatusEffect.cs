using UnityEngine;

public abstract class StatusEffect : Pickable
{
    public override PickableType PickableType => PickableType.StatusEffect;

    [SerializeField] protected int numberOfTicks;

    public StatusEffectList OnList { get; set; }
    public Health OnHealth => OnList.Health;
    public bool IsServer => OnList.isServer;
    public bool IsLocalPlayer => OnList.isLocalPlayer;

    public int NumberOfTicksRemaining { get; set; }

    public void OnPickup()
    {
        NumberOfTicksRemaining = numberOfTicks;
        InnerOnPickup();
    }

    protected abstract void InnerOnPickup();

    public abstract void OnDrop();

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
