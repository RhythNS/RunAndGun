using Mirror;

/// <summary>
/// Helper class for sending emotes to the server.
/// </summary>
public class EmoteCommunicator : NetworkBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    /// <summary>
    /// Sends an emote to the server.
    /// </summary>
    /// <param name="emoteID">The emote to be sent.</param>
    [Command]
    public void CmdSend(int emoteID)
    {
        NetworkServer.SendToAll(new EmoteMessage(emoteID, player.playerId));
    }
}
