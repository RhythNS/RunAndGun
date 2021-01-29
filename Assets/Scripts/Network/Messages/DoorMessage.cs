using Mirror;

public struct DoorMessage : NetworkMessage
{
    public int roomId;
    public bool open;
}
