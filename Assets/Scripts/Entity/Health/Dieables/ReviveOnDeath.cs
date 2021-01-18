using UnityEngine;

public class ReviveOnDeath : MonoBehaviour, IDieable
{
    public void Die()
    {
        Health health = GetComponent<Health>();
        if (health.isServer)
        {
            Debug.Log("Revived " + gameObject.name);
            health.Revive(int.MaxValue);
        }
    }
}
