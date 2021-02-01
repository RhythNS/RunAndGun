using System.Collections;
using UnityEngine;

public static class EnumeratorUtil
{
    public static IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}
