using UnityEngine;

public abstract class BulletSpawnModel : ScriptableObject
{
    public abstract void Shoot(Health shooter, EquippedWeapon weapon, Vector3 position, Vector2 direction);
}
