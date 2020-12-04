using UnityEngine;

public abstract class InstantPickup : MonoBehaviour
{
    public void PickUp(Player player)
    {
        InnerPickUp(player);
        Destroy(gameObject);
    }

    protected abstract void InnerPickUp(Player player);
}
