using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// UI Element for displaying messages to the screen.
/// </summary>
public class NotificationManager : MonoBehaviour
{
    [SerializeField] private float timeForSingleNotification = 2.0f;
    [SerializeField] private int maxLines = 5;
    [SerializeField] private TMP_Text text;

    private List<Tuple<string, ExtendedCoroutine>> displayingStrings;

    private void Start()
    {
        displayingStrings = new List<Tuple<string, ExtendedCoroutine>>();
        text.text = "";
    }

    /// <summary>
    /// Clears all messages.
    /// </summary>
    public void ClearAll()
    {
        if (displayingStrings == null)
            return;

        for (int i = 0; i < displayingStrings.Count; i++)
            displayingStrings[i].Item2.Stop(false);
        displayingStrings = new List<Tuple<string, ExtendedCoroutine>>();
        text.text = "";
    }

    /// <summary>
    /// Shows a message.
    /// </summary>
    /// <param name="toDisplay">The message to be displayed.</param>
    public void Show(string toDisplay)
    {
        displayingStrings.Add(new Tuple<string, ExtendedCoroutine>(toDisplay,
            new ExtendedCoroutine(this, StartTimerForDeletion(), startNow: true)));

        if (displayingStrings.Count >= maxLines)
        {
            displayingStrings[0].Item2.Stop(false);
            displayingStrings.RemoveAt(0);
        }

        UpdateDisplay();
    }

    /// <summary>
    /// Updates the display.
    /// </summary>
    private void UpdateDisplay()
    {
        if (displayingStrings.Count == 0)
        {
            text.text = "";
            return;
        }

        StringBuilder sb = new StringBuilder(displayingStrings[0].Item1);
        for (int i = 1; i < displayingStrings.Count; i++)
        {
            sb.Append(Environment.NewLine);
            sb.Append(displayingStrings[i].Item1);
        }

        text.text = sb.ToString();
    }

    private IEnumerator StartTimerForDeletion()
    {
        yield return new WaitForSeconds(timeForSingleNotification);
        displayingStrings.RemoveAt(0);
        UpdateDisplay();
    }
}
