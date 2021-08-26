using Mirror;
using System.Collections;
using UnityEngine;

/// <summary>
/// Used for playing sounds that only the local player should hear.
/// </summary>
public class LocalSound : NetworkBehaviour
{
    private ExtendedCoroutine weaponCanNotShootCoroutine;

    /// <summary>
    /// Plays a sound that the weapon can not shoot for whatever reason, if it was not play
    /// too long ago.
    /// </summary>
    /// <param name="eventName">The event to be played</param>
    public void PlayWeaponCanNotShoot(string eventName)
    {
        if (weaponCanNotShootCoroutine != null && weaponCanNotShootCoroutine.IsFinshed == false)
            return;

        weaponCanNotShootCoroutine = new ExtendedCoroutine(this, WeaponCanNotShoot(eventName), startNow: true);
    }

    private IEnumerator WeaponCanNotShoot(string eventName)
    {
        FMODUtil.PlayOnTransform(eventName, transform);
        yield return new WaitForSeconds(0.5f);
    }
}
