using System.Collections;
using UnityEngine;

/// <summary>
/// Manages how and when bullets are spawned when the weapon wants to fire.
/// </summary>
public abstract class ShotModel : ScriptableObject
{
    /// <summary>
    /// Coroutine called when the weapon is supposed to be firing.
    /// </summary>
    /// <param name="equipped">A reference to the weapon on the entity.</param>
    public abstract IEnumerator Shoot(EquippedWeapon equipped);
}
