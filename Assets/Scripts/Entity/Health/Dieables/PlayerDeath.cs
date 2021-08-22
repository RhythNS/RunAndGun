using UnityEngine;

/// <summary>
/// Used for when a player died.
/// </summary>
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
