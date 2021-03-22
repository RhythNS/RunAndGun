using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pickable/Weapon/BulletPath/Straight")]
public class BulletPath : ScriptableObject
{
    public virtual float GetCurrentAngle(float aliveTime) {
        return 0f;
    }
}
