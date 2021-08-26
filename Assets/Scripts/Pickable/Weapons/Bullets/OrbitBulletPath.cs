using UnityEngine;

/// <summary>
/// Makes the bullet orbit around the shooter.
/// </summary>
[CreateAssetMenu(menuName = "Pickable/Weapon/BulletPath/Orbit")]
public class OrbitBulletPath : BulletPath
{
    [SerializeField]
    [Range(1f, 115f)]
    private float radius = 1f;

    public override float GetCurrentAngle(float aliveTime)
    {
        float newAngle = Mathf.Rad2Deg * aliveTime * radius;
        return newAngle;
    }
}
