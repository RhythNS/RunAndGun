using UnityEngine;

/// <summary>
/// Adds money to the players inventory when picked up.
/// </summary>
[CreateAssetMenu(menuName = "Pickable/Consumable/Money")]
public class Money : Consumable
{
    [SerializeField] private int moneyAmount;

    public override void Affect(Player player)
    {
        player.Inventory.money += moneyAmount;
    }
}
