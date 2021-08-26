/// <summary>
/// Helper class that can be inherrited from, if not every method of Item is needed.
/// </summary>
public class ItemAdapter : Item
{
    public override void OnDrop(Player player)
    {
    }

    public override void OnHold(Player player, ref StatsEffect statsEffect)
    {
    }

    public override void OnPickUp(Player player)
    {
    }
}
