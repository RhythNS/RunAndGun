using UnityEngine;

/// <summary>
/// Manages incoming emote messages and displays corresponding emotes.
/// </summary>
public class EmoteBoard : MonoBehaviour
{
    [SerializeField] private EmoteMessageDisplay emoteMessagePrefab;

    /// <summary>
    /// How long the emote is alive for.
    /// </summary>
    public float MessageAliveTime => messageAliveTime;
    [SerializeField] private float messageAliveTime;

    /// <summary>
    /// The enter and exit animation of the emote.
    /// </summary>
    public AnimationCurve ScaleCurve => scaleCurve;
    [SerializeField] private AnimationCurve scaleCurve;

    /// <summary>
    /// Called when an emote message was recieved.
    /// </summary>
    /// <param name="emoteMessage">The recieved emote message.</param>
    public void OnPlayerEmoted(EmoteMessage emoteMessage)
    {
        RectTransform rectTransform = transform as RectTransform;
        EmoteMessageDisplay messageDisplay = Instantiate(emoteMessagePrefab, transform);
        ((RectTransform)(messageDisplay.transform)).anchoredPosition 
            =  MathUtil.RandomVector2(-rectTransform.sizeDelta * 0.5f, rectTransform.sizeDelta * 0.5f);
        messageDisplay.Set(emoteMessage, this);
    }

    /// <summary>
    /// Clears all emotes.
    /// </summary>
    public void ClearAll()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
