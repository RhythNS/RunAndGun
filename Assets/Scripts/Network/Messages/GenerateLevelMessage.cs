using Mirror;

/// <summary>
/// Message for when a new level is supposed to be generated.
/// </summary>
public struct GenerateLevelMessage : NetworkMessage
{
    public int levelNumber;
    public Region region;
}
