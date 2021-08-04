using TMPro;
using UnityEngine;

public class GameOverStat : MonoBehaviour
{
    [SerializeField] private TMP_Text statName;
    [SerializeField] private TMP_Text statValue;

    public void Set(Stat stat)
    {
        statName.text = stat.Name;
        statValue.text = stat.StringValue;
    }
}
