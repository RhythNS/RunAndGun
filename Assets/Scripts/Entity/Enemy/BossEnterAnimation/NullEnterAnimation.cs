using System.Collections;
using UnityEngine;

/// <summary>
/// Debug animation for instanly playing the animation.
/// </summary>
public class NullEnterAnimation : BossEnterAnimation
{
    public override AnimationType Type => AnimationType.Null;

    public override IEnumerator PlayAnimation(GameObject boss, BossRoom room)
    {
        yield break;
    }
}
