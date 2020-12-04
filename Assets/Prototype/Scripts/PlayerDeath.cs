using UnityEngine;

public class PlayerDeath : IDieable
{
    [SerializeField] private Respawn respawn;
    public override void Die()
    {
        Destroy(GetComponent<Health>());
        GetComponent<Player>().enabled = false;

        respawn.gameObject.SetActive(true);
    }
}
