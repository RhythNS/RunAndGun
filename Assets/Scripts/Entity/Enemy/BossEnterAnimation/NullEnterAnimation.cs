using System.Collections;
using UnityEngine;

public class NullEnterAnimation : BossEnterAnimation
{
    public override AnimationType Type => AnimationType.Null;

    public override IEnumerator PlayAnimation(GameObject boss, BossRoom room)
    {
        yield break;
    }
}
