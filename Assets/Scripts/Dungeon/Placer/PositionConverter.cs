using UnityEngine;

/// <summary>
/// Helper class for setting the z position of gameobjects to ensure correct rendering ordering.
/// </summary>
public static class PositionConverter
{
    public static float invMapMaxY = 1.0f / 1000.0f;
    public static float maxZ = 250.0f;

    /// <summary>
    /// Sets the z position based on positions y position.
    /// </summary>
    public static void AdjustZ(ref Vector3 position)
    {
        position.z = position.y * invMapMaxY * maxZ;
    }

    /// <summary>
    /// Sets the z position based on positions y position.
    /// </summary>
    public static void AdjustZ(Transform transform)
    {
        Vector3 position = transform.position;
        position.z = position.y * invMapMaxY * maxZ;
        transform.position = position;
    }
}
