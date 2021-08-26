using UnityEngine;

/// <summary>
/// Determines what happens when a bullet is suppossed to be spawned.
/// </summary>
public abstract class BulletSpawnModel : ScriptableObject
{
    /// <summary>
    /// Gets the accuracy angle that the child classes can use.
    /// </summary>
    /// <param name="accuracy">How accurate the shot should be.</param>
    /// <returns>The random angle calculated.</returns>
    protected float GetAccuracyAngle(float accuracy)
    {
        return (1f - (accuracy / 100f)) * Random.Range(-45f, 45f);
    }

    /// <summary>
    /// Called when a bullet should spawn.
    /// </summary>
    /// <param name="weapon">A reference to the weapon equipped on an entity.</param>
    public void Shoot(EquippedWeapon weapon)
    {
        weapon.OnFiredSingleShot();
        InnerShoot(weapon);
    }

    /// <summary>
    /// Called when a bullet should spawn.
    /// </summary>
    /// <param name="weapon">A reference to the weapon equipped on an entity.</param>
    protected abstract void InnerShoot(EquippedWeapon weapon);
}
