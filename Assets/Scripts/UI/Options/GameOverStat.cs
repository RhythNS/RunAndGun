using TMPro;
using UnityEngine;

/// <summary>
/// Displays a stat for the game over screen.
/// </summary>
public class GameOverStat : MonoBehaviour
{
    [SerializeField] private TMP_Text statName;
    [SerializeField] private TMP_Text statValue;

    /// <summary>
    /// Sets the name and value of the stat.
    /// </summary>
    /// <param name="stat">The stat gotten from the stats transmission</param>
    public void Set(string stat)
    {
        string[] seperated = stat.Split(StatsTransmission.SEPERATOR);
        statName.text = seperated[0];
        statValue.text = seperated[1];
    }
}
