using Mirror;
using System.Collections;
using UnityEngine;

/// <summary>
/// Destroys the entity on death.
/// </summary>
public class DestroyOnDeath : MonoBehaviour, IDieable
{
    public void Die()
    {
        Health health = GetComponent<Health>();
        if (health.isServer)
        {
            StartCoroutine(WaitAndDie());
        }
    }

    private IEnumerator WaitAndDie()
    {
        yield return null;
        NetworkServer.Destroy(gameObject);
    }
}
