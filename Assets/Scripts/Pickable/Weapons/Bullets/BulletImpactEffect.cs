using UnityEngine;

/// <summary>
/// Effect that plays when a bullet hits something.
/// </summary>
public abstract class BulletImpactEffect : ScriptableObject
{
    /// <summary>
    /// Bullet impacted something.
    /// </summary>
    /// <param name="position">The position of the impact.</param>
    /// <param name="inflicter">The entity who shoot the bullet.</param>
    /// <param name="hitHealth">Wheter it hit another entity.</param>
    public abstract void OnBulletImpacted(Vector3 position, Health inflicter, bool hitHealth);
}
