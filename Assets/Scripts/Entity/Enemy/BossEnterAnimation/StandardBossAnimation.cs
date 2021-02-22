using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardBossAnimation : BossEnterAnimation
{
    public override AnimationType Type => AnimationType.Standard;

    public override IEnumerator PlayAnimation(GameObject boss, BossRoom room)
    {
        throw new System.NotImplementedException();
    }
}
