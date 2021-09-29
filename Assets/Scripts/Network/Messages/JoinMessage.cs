using Mirror;

/// <summary>
/// Message is sent when a player joins or changes his character.
/// </summary>
public struct JoinMessage : NetworkMessage
{
    public CharacterType characterType;
    public string name;
    public string password;
    public string uniqueIdentifier;

    public static JoinMessage GetDefault()
    {
        return new JoinMessage
        {
            name = Config.Instance.PlayerName,
            characterType = Config.Instance.SelectedPlayerType,
            password = Config.Instance.password,
            uniqueIdentifier = Config.Instance.GetUniqueIdentifier()
        };
    }
}
