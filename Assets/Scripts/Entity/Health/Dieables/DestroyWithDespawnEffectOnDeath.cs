using Mirror;
using UnityEngine;

/// <summary>
/// Destroys the entity after the despawn effect played.
/// </summary>
public class DestroyWithDespawnEffectOnDeath : MonoBehaviour, IDieable
{
    public void Die()
    {
        Health health = GetComponent<Health>();
        EntityMaterialManager emm = GetComponent<EntityMaterialManager>();
        Enemy enemy = GetComponent<Enemy>();

        if (health.isServer)
        {
            if (enemy)
                enemy.Brain.enabled = false;
            emm.PlayDeSpawnEffect(onFinished: Despawn);
        }
        else
        {
            emm.PlayDeSpawnEffect(onFinished: null);
        }

        health.enabled = false;
        gameObject.layer = LayerDict.Instance.GetUnhittableLayer();
    }

    private void Despawn()
    {
        NetworkServer.Destroy(gameObject);
    }
}
