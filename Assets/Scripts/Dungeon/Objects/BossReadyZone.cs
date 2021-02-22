using UnityEngine;

public class BossReadyZone : MonoBehaviour
{
    public void SetBorder(Rect rect)
    {
        transform.position = rect.position;
        transform.localScale = rect.size;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player) == false)
            return;

        if (Player.LocalPlayer == null || Player.LocalPlayer.isServer)
            DungeonDict.Instance.BossRoom.OnPlayerReadyToEnterChanged(player, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player) == false)
            return;

        if (Player.LocalPlayer == null || Player.LocalPlayer.isServer)
            DungeonDict.Instance.BossRoom.OnPlayerReadyToEnterChanged(player, false);
    }
}
