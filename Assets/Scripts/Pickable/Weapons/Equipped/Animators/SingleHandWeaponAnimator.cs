using UnityEngine;

public class SingleHandWeaponAnimator : WeaponAnimator
{
    public override WeaponAnimatorType WeaponAnimatorType => WeaponAnimatorType.SingleHand;

    public override void SetDirection(Vector2 direction)
    {
        if (direction.x > 0.0f)
        {
            transform.localPosition = WeaponPoints.EastHand;
            SpriteRenderer.flipX = false;
        }
        else
        {
            transform.localPosition = WeaponPoints.WestHand;
            SpriteRenderer.flipX = true;
        }
        float angle = Vector2.Angle(Vector2.right, direction);
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
    }
}
