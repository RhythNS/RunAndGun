using UnityEngine;

/// <summary>
/// Picks up all Pickables which should be automaticly picked up, when moved on them.
/// </summary>
public class AutoPickup : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PickableInWorld pickable)
            && pickable.Pickable.InstantPickup)
            player.CmdPickup(pickable.gameObject);
    }
}
