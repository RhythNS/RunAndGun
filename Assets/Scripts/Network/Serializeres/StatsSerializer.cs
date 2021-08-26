using Mirror;
using System;
using System.Collections.Generic;

/// <summary>
/// Helper class to send and recieve stats.
/// </summary>
[System.Serializable]
public class StatsTransmission
{
    public static readonly char SEPERATOR = ';';

    public string[] names;
    public CharacterType[] characterTypes;
    public string[][] stats;

    public StatsTransmission(Dictionary<Player, Dictionary<Type, Stat>> statsForPlayer)
    {
        names = new string[statsForPlayer.Count];
        characterTypes = new CharacterType[statsForPlayer.Count];
        stats = new string[statsForPlayer.Count][];

        int i = 0;
        foreach (Player player in statsForPlayer.Keys)
        {
            names[i] = player.entityName;
            characterTypes[i] = player.CharacterType;

            Dictionary<Type, Stat> playerStats = statsForPlayer[player];
            stats[i] = new string[playerStats.Count];
            int j = 0;
            foreach (Type type in playerStats.Keys)
            {
                stats[i][j] = playerStats[type].Name + SEPERATOR + playerStats[type].StringValue;
                j++;
            }
            ++i;
        }
    }

    public StatsTransmission(string[] names, CharacterType[] characterTypes, string[][] stats)
    {
        this.names = names;
        this.characterTypes = characterTypes;
        this.stats = stats;
    }
}

/// <summary>
/// Serializers for reading and writing StatsTransmissions.
/// </summary>
public static class StatsSerializer
{
    public static StatsTransmission ReadStatsTransmission(this NetworkReader reader)
    {
        string[] names = reader.ReadArray<string>();

        int[] readCharacterTypes = reader.ReadArray<int>();
        CharacterType[] types = new CharacterType[readCharacterTypes.Length];
        for (int i = 0; i < types.Length; i++)
            types[i] = (CharacterType)readCharacterTypes[i];

        string[][] stats = new string[names.Length][];
        for (int i = 0; i < names.Length; i++)
        {
            stats[i] = reader.ReadArray<string>();
        }

        return new StatsTransmission(names, types, stats);
    }

    public static void WriteStatsTransmission(this NetworkWriter writer, StatsTransmission stats)
    {
        writer.WriteArray(stats.names);

        int[] characterTypes = new int[stats.characterTypes.Length];
        for (int i = 0; i < characterTypes.Length; i++)
            characterTypes[i] = (int)stats.characterTypes[i];
        writer.WriteArray(characterTypes);

        for (int i = 0; i < stats.stats.Length; i++)
        {
            writer.WriteArray(stats.stats[i]);
        }
    }
}
