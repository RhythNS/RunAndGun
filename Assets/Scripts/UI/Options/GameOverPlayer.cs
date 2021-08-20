using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPlayer : MonoBehaviour
{
    [SerializeField] private TMP_Text nameDisplay;
    [SerializeField] private Image playerIcon;
    [SerializeField] private RectTransform contentTrans;

    public void Set(string name, CharacterType type, string[] stats)
    {
        nameDisplay.text = name;

        playerIcon.sprite = CharacterDict.Instance.GetSpriteForType(type);

        foreach (string stat in stats)
        {
            GameOverStat gos = Instantiate(GameOverScreen.Instance.statPrefab, contentTrans);
            gos.Set(stat);
        }
    }
}
