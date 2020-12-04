using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUpdater : MonoBehaviour
{
    private int amount = 0;
    private TMP_Text text;
    private static ScoreUpdater instance;

    private void Awake()
    {
        instance = this;
        text = GetComponent<TMP_Text>();
    }

    public static void OnEnemyKilled()
    {
        instance.text.text = "Score: " + ++instance.amount;
    }

    public static int GetScore() => instance.amount;
}
