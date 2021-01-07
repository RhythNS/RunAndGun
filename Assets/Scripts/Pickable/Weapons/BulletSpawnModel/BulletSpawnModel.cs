using UnityEngine;

public abstract class BulletSpawnModel : ScriptableObject
{
    public void Shoot(EquippedWeapon weapon)
    {
        weapon.OnFiredSingleShot();
        InnerShoot(weapon);
    }

    protected abstract void InnerShoot(EquippedWeapon weapon);
}
