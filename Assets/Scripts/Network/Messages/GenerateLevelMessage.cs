using Mirror;

public struct GenerateLevelMessage : NetworkMessage
{
    public int levelNumber;
    public Region region;
}
