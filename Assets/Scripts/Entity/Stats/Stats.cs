using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : NetworkBehaviour
{
    // TODO: Remove SerializeField
    [SyncVar] [SerializeField] private int baseHealth;
    [SyncVar] [SerializeField] private int health;
    [SyncVar] [SerializeField] private int baseDefence;
    [SyncVar] [SerializeField] private int defence;
    [SyncVar] [SerializeField] private int baseSpeed;
    [SyncVar] [SerializeField] private int speed;
    [SyncVar] [SerializeField] private int baseLuck;
    [SyncVar] [SerializeField] private int luck;
    [SyncVar] [SerializeField] private int baseDodge;
    [SyncVar] [SerializeField] private int dodge;

    private void Awake()
    {
        health = baseHealth;
        defence = baseDefence;
        speed = baseSpeed;
        luck = baseLuck;
        dodge = baseDodge;
    }

    public float GetMovementForce()
    {
        // TODO: Some fitting formula
        return speed * 600;
    }

    public float GetDodgeCooldown()
    {
        // TODO: Some fitting formula
        return 3.0f * (1 - (float)(Mathf.Max(1, 10 - dodge) / 10));
    }
}
