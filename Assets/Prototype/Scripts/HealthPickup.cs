using UnityEngine;

public class HealthPickup : InstantPickup
{
    [SerializeField] private int healAmount;

    protected override void InnerPickUp(Player player)
    {
        player.GetComponent<Health>().Damage(-healAmount);
    }
}
