using Mirror;
using UnityEngine;

public struct BossSpawnMessage : NetworkMessage
{
    public int id;
    public GameObject[] bossGameObjects;
    public BossEnterAnimation.AnimationType animationType;

    public BossSpawnMessage(int id, GameObject[] bossGameObjects, BossEnterAnimation.AnimationType animationType)
    {
        this.id = id;
        this.bossGameObjects = bossGameObjects;
        this.animationType = animationType;
    }
}
