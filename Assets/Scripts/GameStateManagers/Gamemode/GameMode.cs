using MapGenerator;
using UnityEngine;

[CreateAssetMenu(menuName = "Gamemode/Gamemode")]
public class GameMode : ScriptableObject
{
    [HideInInspector] public int[] levelSeeds;

    public int levelAmount;
    //public GameRule[] gameRules;
    public int startingDifficulty;
    public int minPlayers, maxPlayers;
    public bool randomSeed;
    public int seed;
    public DungeonConfig dungeonConfig;
    public RegionOnLevel[] customRegionOnLevels;

    [System.Serializable]
    public struct RegionOnLevel
    {
        public int level;
        public Region region;
    }

    public void Init(int seed)
    {
        Random.InitState(seed);
        levelSeeds = new int[levelAmount];
        for (int i = 0; i < levelAmount; i++)
            levelSeeds[i] = Random.Range(int.MinValue, int.MaxValue);
    }

    public bool CustomRegionsAreValid(out string errorReason)
    {
        errorReason = "";

        if (customRegionOnLevels == null || customRegionOnLevels.Length == 0)
            return true;

        if (customRegionOnLevels[0].level != 0)
        {
            errorReason = "The first custom region must start at level 0.";
            return false;
        }

        for (int i = 1; i < customRegionOnLevels.Length; i++)
        {
            if (customRegionOnLevels[i - 1].level > customRegionOnLevels[i].level)
            {
                errorReason = "Every custom region must have a higher level than the previous region.";
                return false;
            }
        }

        return true;
    }
}
