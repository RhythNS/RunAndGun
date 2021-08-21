using UnityEngine;

public abstract class Effect : ScriptableObject
{
    public abstract void OnHit(Weapon weapon, Health affecter, Health health);
}
