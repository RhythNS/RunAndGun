using Mirror;

/// <summary>
/// Message when a player emoted.
/// </summary>
public struct EmoteMessage : NetworkMessage
{
    public int emoteID;
    public int playerID;

    public EmoteMessage(int emoteID, int playerID)
    {
        this.emoteID = emoteID;
        this.playerID = playerID;
    }
}
