using TMPro;
using UnityEngine;

public class GameOverStat : MonoBehaviour
{
    [SerializeField] private TMP_Text statName;
    [SerializeField] private TMP_Text statValue;

    public void Set(string stat)
    {
        string[] seperated = stat.Split(StatsTransmission.SEPERATOR);
        statName.text = seperated[0];
        statValue.text = seperated[1];
    }
}
