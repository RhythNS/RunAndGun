using System.Collections;
using UnityEngine;

public abstract class ShotModel : ScriptableObject
{
    public abstract IEnumerator Shoot(Health shooter, EquippedWeapon equipped, Vector3 position, Vector2 direction);
}
