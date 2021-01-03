using UnityEngine;

public abstract class BulletSpawnModel : ScriptableObject
{
    public abstract void Shoot(Health shooter, Bullet bullet, Vector3 position, Vector2 direction);
}
