using Mirror;

public struct DisconnectMessage : NetworkMessage
{
    public enum Type
    {
        Unknown = 0, PasswordWrong, ServerFull, Kicked, JoinedGameInProgress
    }

    public Type type;
}
