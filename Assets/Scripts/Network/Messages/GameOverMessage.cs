using Mirror;

/// <summary>
/// Message is sent when every player died.
/// </summary>
public struct GameOverMessage : NetworkMessage
{
    public StatsTransmission statsTransmission;

    public GameOverMessage(StatsTransmission statsTransmission)
    {
        this.statsTransmission = statsTransmission;
    }
}
