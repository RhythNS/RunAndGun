using System.Collections;
using UnityEngine;

/// <summary>
/// Abstraction of enter animation for bosses.
/// </summary>
public abstract class BossEnterAnimation : MonoBehaviour
{
    public enum AnimationType
    {
        Null, Standard
    }

    /// <summary>
    /// Adds the Animation to the boss object.
    /// </summary>
    /// <param name="toAddTo">The boss to where the animation should be added to.</param>
    /// <param name="type">The type of animation to be added to.</param>
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

    /// <summary>
    /// Plays the animation.
    /// </summary>
    /// <param name="boss">The spawned boss.</param>
    /// <param name="room">The room where the boss spawned.</param>
    public abstract IEnumerator PlayAnimation(GameObject boss, BossRoom room);
}
