using System.Collections;
using UnityEngine;

public abstract class ShotModel : ScriptableObject
{
    public abstract IEnumerator Shoot(EquippedWeapon equipped);
}
