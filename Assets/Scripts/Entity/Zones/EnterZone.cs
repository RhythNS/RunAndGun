using UnityEngine;

public abstract class EnterZone : MonoBehaviour
{
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

    public abstract void OnEnter(Player player);

    public abstract void OnExit(Player player);
}
