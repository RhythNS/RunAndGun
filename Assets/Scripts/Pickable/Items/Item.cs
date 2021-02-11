public abstract class Item : Pickable
{
    public StatusEffectList OnList { get; set; }

    public override PickableType PickableType => PickableType.Item;

    public abstract void OnPickUp(Player player);

    public abstract void OnHold(Player player, ref StatsEffect statsEffect);

    public abstract void OnDrop(Player player);
}
