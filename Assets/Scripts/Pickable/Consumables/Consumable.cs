/// <summary>
/// A pickable that is instaly picked up and is not added to a players inventory.
/// </summary>
public abstract class Consumable : Pickable
{
    public override PickableType PickableType => PickableType.Consumable;

    /// <summary>
    /// Called when the consumable has been picked up.
    /// </summary>
    /// <param name="player">The player that picked the consumable up.</param>
    public abstract void Affect(Player player);
}
