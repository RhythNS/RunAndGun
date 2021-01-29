using UnityEngine;

public class ReadyZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player) && player == Player.LocalPlayer)
            player.StateCommunicator.CmdLobbySetReady(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player) && player == Player.LocalPlayer)
            player.StateCommunicator.CmdLobbySetReady(false);
    }
}
