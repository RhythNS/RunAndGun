using UnityEngine;

public class NullWeaponAnimator : WeaponAnimator
{
    public override WeaponAnimatorType WeaponAnimatorType => WeaponAnimatorType.Null;

    public override void OnSingleShotFired() { }

    public override void OnStartedFire() { }

    public override void OnStartedReload() { }

    public override void OnStoppedFire() { }

    public override void OnStoppedReload() { }

    public override void SetDirection(Vector2 direction) { }
}
