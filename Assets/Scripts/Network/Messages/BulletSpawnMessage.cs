using Mirror;
using UnityEngine;

/// <summary>
/// Message for when a bullet was spawned.
/// </summary>
public struct BulletSpawnMessage : NetworkMessage
{
    public GameObject shooter;
    public Vector2 initialVelocity;
    public Vector2 initialPosition;
}
