using UnityEngine;

[CreateAssetMenu(menuName = "Pickable/Item/Stats Item")]
public class StatsItem : Item
{
    [SerializeField] private StatsEffect effect;

    public override void OnDrop(Player player)
    {
    }

    public override void OnHold(Player player, ref StatsEffect statsEffect)
    {
        statsEffect.Add(effect);
    }

    public override void OnPickUp(Player player)
    {
    }
}
