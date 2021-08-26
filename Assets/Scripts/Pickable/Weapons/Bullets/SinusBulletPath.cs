using UnityEngine;

/// <summary>
/// Makes the bullet fly on a sinus path.
/// </summary>
[CreateAssetMenu(menuName = "Pickable/Weapon/BulletPath/SinusCurve")]
public class SinusBulletPath : BulletPath
{
    [SerializeField][Range(0.1f, 100f)]
    private float frequency = 1f;

    [SerializeField][Range(0f, 1f)]
    private float startTime = 0f;

    public override float GetCurrentAngle(float aliveTime)
    {
        float newAngle = Mathf.Rad2Deg * Mathf.Sin(frequency * (startTime + aliveTime) - Mathf.PI * 0.5f);
        return newAngle;
    }
}
