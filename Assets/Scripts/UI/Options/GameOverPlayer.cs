using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPlayer : MonoBehaviour
{
    [SerializeField] private TMP_Text nameDisplay;
    [SerializeField] private Image playerIcon;
    [SerializeField] private RectTransform contentTrans;

    public void Set(Player player, Dictionary<Type, Stat> stats)
    {
        nameDisplay.text = player.entityName;

        foreach (Stat stat in stats.Values)
        {
            GameOverStat gos = Instantiate(GameOverScreen.Instance.statPrefab, contentTrans);
            gos.Set(stat);
        }
    }
}
