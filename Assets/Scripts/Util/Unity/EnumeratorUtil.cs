using System.Collections;
using UnityEngine;

public static class EnumeratorUtil
{
    public static IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    public static IEnumerator Sequence(params IEnumerator[] enumerators)
    {
        for (int i = 0; i < enumerators.Length; i++)
        {
            yield return enumerators[i];
        }
    }

    public static IEnumerator GoToInSecondsLerp(Transform transform, Vector2 position, float seconds)
    {
        Vector3 oldPos = transform.position;
        Vector3 newPos = position;
        newPos.z = oldPos.z;

        float timer = 0;
        do
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(oldPos, newPos, timer / seconds);
            yield return null;
        } while (timer < seconds);
    }

    public static IEnumerator GoToInSecondsSlerp(Transform transform, Vector2 position, float seconds)
    {
        Vector3 oldPos = transform.position;
        Vector3 newPos = position;
        newPos.z = oldPos.z;

        float timer = 0;
        do
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Slerp(oldPos, newPos, timer / seconds);
            yield return null;
        } while (timer < seconds);
    }

    public static IEnumerator GoToInSecondsCurve(Transform transform, Vector2 position, AnimationCurve curve, float seconds)
    {
        Vector3 oldPos = transform.position;
        Vector3 newPos = position;
        newPos.z = oldPos.z;

        float timer = 0;
        do
        {
            timer += Time.deltaTime;
            transform.position = Vector3.LerpUnclamped(oldPos, newPos, curve.Evaluate(timer / seconds));
            yield return null;
        } while (timer < seconds);
    }

    public static IEnumerator GoToInSecondsLocalCurve(Transform transform, Vector2 position, AnimationCurve curve, float seconds)
    {
        Vector3 oldPos = transform.localPosition;
        Vector3 newPos = position;
        newPos.z = oldPos.z;

        float timer = 0;
        do
        {
            timer += Time.deltaTime;
            transform.localPosition = Vector3.LerpUnclamped(oldPos, newPos, curve.Evaluate(timer / seconds));
            yield return null;
        } while (timer < seconds);
    }

    public static IEnumerator FadeGroupCurve(CanvasGroup group, AnimationCurve curve, float seconds, bool inverted = false)
    {
        float timer = 0;
        do
        {
            timer += Time.deltaTime;
            float eval = curve.Evaluate(timer / seconds);
            group.alpha = inverted ? 1 - eval : eval;
            yield return null;
        } while (timer < seconds);
    }
}
