using UnityEngine;

/// <summary>
/// Generic zone that calls OnEnter or OnExit when the local player entered or exited it.
/// </summary>
public abstract class EnterZone : MonoBehaviour
{
    private void Start()
    {
        PositionConverter.AdjustZ(transform);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player) && player == Player.LocalPlayer)
            OnEnter(player);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player) && player == Player.LocalPlayer)
            OnExit(player);
    }

    /// <summary>
    /// Called when the local player entered this zone.
    /// </summary>
    /// <param name="player">The local player that entered.</param>
    public abstract void OnEnter(Player player);

    /// <summary>
    /// Called when the local player exited this zone.
    /// </summary>
    /// <param name="player">The local player that exited.</param>
    public abstract void OnExit(Player player);
}
