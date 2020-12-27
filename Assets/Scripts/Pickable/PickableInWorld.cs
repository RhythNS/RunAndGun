using Mirror;
using UnityEngine;

public class PickableInWorld : NetworkBehaviour
{
    public Pickable Pickable => pickable;
    [SyncVar] private Pickable pickable;

    public static void Place(Pickable pickable, Vector3 position)
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Pickable.InstantPickup && collision.gameObject.TryGetComponent(out Player player)
            && player.isLocalPlayer)
            player.CmdPickup(gameObject);
    }
}
