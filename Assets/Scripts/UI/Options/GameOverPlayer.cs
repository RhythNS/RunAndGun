using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the display of all stats of a single player for the game over screen.
/// </summary>
public class GameOverPlayer : MonoBehaviour
{
    [SerializeField] private TMP_Text nameDisplay;
    [SerializeField] private Image playerIcon;
    [SerializeField] private RectTransform contentTrans;

    /// <summary>
    /// Displays all stats.
    /// </summary>
    /// <param name="name">The name of the player.</param>
    /// <param name="type">The type of character the player played.</param>
    /// <param name="stats">All stats of the player.</param>
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
