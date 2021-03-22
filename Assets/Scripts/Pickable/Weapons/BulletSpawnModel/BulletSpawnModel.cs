using UnityEngine;

public abstract class BulletSpawnModel : ScriptableObject
{
    protected float GetAccuracyAngle(float accuracy) {
        return (1f - (accuracy / 100f)) * Random.Range(-45f, 45f);
    }

    public void Shoot(EquippedWeapon weapon)
    {
        weapon.OnFiredSingleShot();
        InnerShoot(weapon);
    }

    protected abstract void InnerShoot(EquippedWeapon weapon);
}
