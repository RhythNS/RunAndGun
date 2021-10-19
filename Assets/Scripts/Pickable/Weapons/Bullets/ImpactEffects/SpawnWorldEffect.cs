using Mirror;
using UnityEngine;

/// <summary>
/// Adapter to spawn a world effect when the bullet impacted something.
/// </summary>
[CreateAssetMenu(menuName = "Pickable/Weapon/BulletImpact/SpawnWorldEffect")]
public class SpawnWorldEffect : BulletImpactEffect
{
    [SerializeField] private WorldEffectInWorld prefab;
    [SerializeField] private WorldEffect[] effects;

    public override void OnBulletImpacted(Vector3 position, Health inflicter, bool hitHealth)
    {
        if (NetworkServer.active == false)
            return;

        WorldEffectInWorld.Place(prefab, effects, inflicter, position, Quaternion.identity);
    }
}
