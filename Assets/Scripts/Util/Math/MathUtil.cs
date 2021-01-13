using UnityEngine;

public abstract class MathUtil
{
    public static bool InRangeInclusive(float min, float max, float val)
        => val >= min && val <= max;

    public static bool InRangeInclusive(int min, int max, int val)
        => val >= min && val <= max;

    public static bool InRange(float min, float max, float val)
        => val > min && val < max;

    public static bool InRange(int min, int max, int val)
        => val > min && val < max;

    public static float Normalize(float value, float min, float max)
        => min == 0 ? (value / max) : (value - min) / (max - min);
}
