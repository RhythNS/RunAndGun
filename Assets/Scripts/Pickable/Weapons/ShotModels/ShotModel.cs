using System.Collections;
using UnityEngine;

public abstract class ShotModel : ScriptableObject
{
    public IEnumerator Shoot(EquippedWeapon equipped)
    {
        yield return InnerShoot(equipped);
        equipped.OnStopFiring();
    }

    protected abstract IEnumerator InnerShoot(EquippedWeapon equipped);
}
