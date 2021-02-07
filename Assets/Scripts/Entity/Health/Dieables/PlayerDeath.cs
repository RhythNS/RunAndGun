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
        Debug.Log(player.name + " has died!");
        player.Collider2D.isTrigger = true;
        player.gameObject.layer = LayerDict.Instance.GetDownedPlayerLayer();
        player.PlayerAnimationController?.OnDeath();
    }
}
