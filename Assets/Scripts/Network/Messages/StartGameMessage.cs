using Mirror;

/// <summary>
/// Message is sent when the game is about to start.
/// </summary>
public class StartGameMessage : NetworkMessage
{
    /// <summary>
    /// The seed of the level.
    /// </summary>
    public float levelSeed;
}
