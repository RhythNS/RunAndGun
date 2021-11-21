using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayUtil
{
    public static bool InRange<T>(T[] array, int index) => index > -1 && index < array.Length;
}
