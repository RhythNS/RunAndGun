using System;

/// <summary>
/// Serilizable save game.
/// </summary>
[System.Serializable]
public class SaveGame
{
    public string playerName;
    public CharacterType lastSelectedCharacterType;
    public Tuple<string, float>[] volumes;
}
