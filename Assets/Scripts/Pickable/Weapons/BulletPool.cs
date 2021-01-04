using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : Pool<Bullet>
{
    public static BulletPool Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    protected override Bullet Create()
    {
        // TODO: Implement
        throw new System.NotImplementedException();
    }
}
