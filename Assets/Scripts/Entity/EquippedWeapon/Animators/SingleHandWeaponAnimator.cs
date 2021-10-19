using UnityEngine;

/// <summary>
/// Animates single hand weapons.
/// </summary>
public class SingleHandWeaponAnimator : WeaponAnimator
{
    public override WeaponAnimatorType WeaponAnimatorType => WeaponAnimatorType.SingleHand;

    public override void SetDirection(Vector2 direction)
    {
        if (direction.x > 0.0f)
        {
            Vector3 pos = WeaponPoints.EastHand;
            pos.z = transform.localPosition.z;
            transform.localPosition = pos;
            SpriteRenderer.flipX = false;
            float angle = Vector2.Angle(Vector2.right, direction);
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, direction.y > 0 ? angle : -angle);
        }
        else
        {
            Vector3 pos = WeaponPoints.WestHand;
            pos.z = transform.localPosition.z;
            transform.localPosition = pos;
            SpriteRenderer.flipX = true;
            float angle = Vector2.Angle(Vector2.left, direction);
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, direction.y > 0 ? -angle : angle);
        }
    }
}
