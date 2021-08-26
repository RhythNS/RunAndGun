using UnityEngine;

/// <summary>
/// Handles how the bullets should fly when shoot.
/// </summary>
[CreateAssetMenu(menuName = "Pickable/Weapon/BulletPath/Straight")]
public class BulletPath : ScriptableObject
{
    public virtual float GetCurrentAngle(float aliveTime)
    {
        return 0f;
    }
}
