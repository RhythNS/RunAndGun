using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    [SerializeField] private float timeForSingleNotification = 2.0f;
    [SerializeField] private int maxLines = 5;
    [SerializeField] private TMP_Text text;

    private List<string> displayingStrings;

    private void Start()
    {
        displayingStrings = new List<string>();
        text.text = "";
    }

    public void Show(string toDisplay)
    {
        displayingStrings.Add(toDisplay);
        UpdateDisplay();
        StartCoroutine(StartTimerForDeletion());
    }

    private void UpdateDisplay()
    {
        if (displayingStrings.Count == 0)
        {
            text.text = "";
            return;
        }

        StringBuilder sb = new StringBuilder(displayingStrings[0]);
        for (int i = 1; i < displayingStrings.Count; i++)
        {
            sb.Append(Environment.NewLine);
            sb.Append(displayingStrings[i]);
        }

        text.text = sb.ToString();
    }

    private IEnumerator StartTimerForDeletion()
    {
        yield return new WaitForSeconds(timeForSingleNotification);
        displayingStrings.RemoveAt(0);
    }
}
