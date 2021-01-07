using Mirror;
using UnityEngine;

public class BulletPool : NetworkPool
{
    public static BulletPool Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    protected override GameObject Create()
    {
        GameObject bullet = Instantiate(prefab, transform);
        return bullet;
    }
}
