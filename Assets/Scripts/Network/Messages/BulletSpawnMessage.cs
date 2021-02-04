using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BulletSpawnMessage : NetworkMessage
{
    public GameObject shooter;
    public Vector2 initialVelocity;
    public Vector2 initialPosition;
}
