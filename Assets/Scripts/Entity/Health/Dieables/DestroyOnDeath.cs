using Mirror;
using UnityEngine;

public class DestroyOnDeath : MonoBehaviour, IDieable
{
    public void Die()
    {
        Health health = GetComponent<Health>();
        if (health.isServer)
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}
