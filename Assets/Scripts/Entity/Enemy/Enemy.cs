using Mirror;
using Smooth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : NetworkBehaviour
{
    public Brain Brain { get; private set; }
    public Health Health { get; private set; }
    public EquippedWeapon EquippedWeapon { get; private set; }
    public SmoothSyncMirror SmoothSync { get; private set; }

    private void Awake()
    {
        Brain = GetComponent<Brain>(); // Should start disabled
        Health = GetComponent<Health>();
        EquippedWeapon = GetComponent<EquippedWeapon>();
        SmoothSync = GetComponent<SmoothSyncMirror>();
    }

    public override void OnStartServer()
    {
        Brain.enabled = true;
    }
}
