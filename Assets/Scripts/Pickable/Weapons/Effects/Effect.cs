using UnityEngine;

/// <summary>
/// Effect for when a bullet hits an entity.
/// </summary>
public abstract class Effect : ScriptableObject
{
    /// <summary>
    /// Called when the bullet hit.
    /// </summary>
    /// <param name="weapon">The weapon from where it was fired from.</param>
    /// <param name="affecter">The health responsbile for the bullet.</param>
    /// <param name="health">The health that is going to be affected by the effect.</param>
    public abstract void OnHit(Weapon weapon, Health affecter, Health health);
}
