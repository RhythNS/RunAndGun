using UnityEngine;

/// <summary>
/// Animates two handed weapons.
/// </summary>
public class TwoHandWeaponAnimator : WeaponAnimator
{
    public override WeaponAnimatorType WeaponAnimatorType => WeaponAnimatorType.TwoHand;

    public override void SetDirection(Vector2 direction)
    {
        SpriteRenderer.flipY = direction.x < 0;
        Vector3 toChange = WeaponPoints.TwoHandedMidPoint + WeaponPoints.Radius * direction;
        toChange.z = transform.localPosition.z;
        transform.localPosition = toChange;
        float angle = Vector2.Angle(Vector2.right, direction);
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, direction.y > 0 ? angle : -angle);
    }
}
