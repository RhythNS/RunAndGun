using Mirror;
using System.Collections;
using UnityEngine;

public class LocalSound : NetworkBehaviour
{
    private ExtendedCoroutine weaponCanNotShootCoroutine;

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
