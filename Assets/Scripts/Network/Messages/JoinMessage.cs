using Mirror;

/// <summary>
/// Message is sent when a player joins or changes his character.
/// </summary>
public struct JoinMessage : NetworkMessage
{
    public CharacterType characterType;
    public string name;
}
