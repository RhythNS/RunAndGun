using UnityEngine;

public abstract class BulletImpactEffect : ScriptableObject
{
    public abstract void OnBulletImpacted(Vector3 position, bool hitHealth);
}
