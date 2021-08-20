using Mirror;

public class EmoteCommunicator : NetworkBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    [Command]
    public void CmdSend(int emoteID)
    {
        NetworkServer.SendToAll(new EmoteMessage(emoteID, player.playerId));
    }
}
