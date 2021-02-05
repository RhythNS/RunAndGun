using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour, IDieable
{
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    public void Die()
    {
        player.PlayerAnimationController.OnDeath();
    }
}
