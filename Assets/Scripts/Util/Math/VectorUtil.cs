﻿using UnityEngine;

/// <summary>
/// Helper methods for vectors.
/// </summary>
public abstract class VectorUtil
{
    /// <summary>
    /// Floors a vector to an integer vector.
    /// </summary>
    /// <param name="input">The vector to be floored.</param>
    /// <returns>The floored vector.</returns>
    public static Vector3Int FloorVectorToInt(in Vector3 input)
        => new Vector3Int(Mathf.FloorToInt(input.x), Mathf.FloorToInt(input.y), Mathf.FloorToInt(input.z));

    public static bool InBounds(Vector2 from, Vector2 to, Vector2 value)
        => value.x >= from.x && value.y >= from.y && value.x < to.x && value.y < to.y;
}
