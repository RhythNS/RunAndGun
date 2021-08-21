﻿using UnityEngine;

/// <summary>
/// Kills the LocalPlayer.
/// </summary>
public class KillSelf : MonoBehaviour
{
    [SerializeField] private Weapon weapon;

    private void Start()
    {
        Debug.Log("Hit self enabled with F3");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
            Player.LocalPlayer.CmdBulletHit(Player.LocalPlayer.gameObject, Player.LocalPlayer.gameObject, weapon);
    }
}
