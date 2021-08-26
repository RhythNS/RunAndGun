using Mirror;

/// <summary>
/// Message for when a new room was uncovered for the minimap.
/// </summary>
public struct MiniMapNewRoomMessage : NetworkMessage
{
    public int roomId;
}
