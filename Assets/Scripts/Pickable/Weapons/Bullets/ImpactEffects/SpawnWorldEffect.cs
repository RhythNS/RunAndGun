using Mirror;
using UnityEngine;

[CreateAssetMenu(menuName = "Pickable/Weapon/BulletImpact/SpawnWorldEffect")]
public class SpawnWorldEffect : BulletImpactEffect
{
    [SerializeField] private WorldEffectInWorld prefab;
    [SerializeField] private WorldEffect[] effects;

    public override void OnBulletImpacted(Vector3 position, Health inflicter, bool hitHealth)
    {
        if (NetworkServer.active == false)
            return;

        WorldEffectInWorld toSpawn = Instantiate(prefab, position, Quaternion.identity);
        toSpawn.Init(inflicter, effects);
        NetworkServer.Spawn(toSpawn.gameObject);
    }
}
