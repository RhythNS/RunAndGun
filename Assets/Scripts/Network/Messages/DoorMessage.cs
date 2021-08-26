using Mirror;

/// <summary>
/// Message for when a door was opened.
/// </summary>
public struct DoorMessage : NetworkMessage
{
    public int roomId;
    public bool open;
}
