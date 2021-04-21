using UnityEngine;

public static class PositionConverter
{
    public static float invMapMaxY = 1.0f / 1000.0f;
    public static float maxZ = 250.0f;

    public static void AdjustZ(ref Vector3 position)
    {
        position.z = position.y * invMapMaxY * maxZ;
    }

    public static void AdjustZ(Transform transform)
    {
        Vector3 position = transform.position;
        position.z = position.y * invMapMaxY * maxZ;
        transform.position = position;
    }
}
