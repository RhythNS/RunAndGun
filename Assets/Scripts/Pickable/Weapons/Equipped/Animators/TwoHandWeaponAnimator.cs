using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoHandWeaponAnimator : WeaponAnimator
{
    public override WeaponAnimatorType WeaponAnimatorType => WeaponAnimatorType.TwoHand;

    public override void SetDirection(Vector2 direction)
    {
        throw new System.NotImplementedException();
    }
}
