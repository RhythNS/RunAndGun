using System.Collections;
using UnityEngine;

public abstract class BossEnterAnimation : MonoBehaviour
{
    public enum AnimationType
    {
        Null, Standard
    }

    public static BossEnterAnimation AddAnimationType(GameObject toAddTo, AnimationType type)
    {
        switch (type)
        {
            case AnimationType.Standard:
                return toAddTo.AddComponent<StandardBossAnimation>();
            case AnimationType.Null:
                return toAddTo.AddComponent<NullEnterAnimation>();
        }
        throw new System.Exception("Type " + type + " not implemented!");
    }

    public abstract AnimationType Type { get; }

    public abstract IEnumerator PlayAnimation(GameObject boss, BossRoom room);
}
