using UnityEngine;

public class EmoteBoard : MonoBehaviour
{
    [SerializeField] private EmoteMessageDisplay emoteMessagePrefab;

    public float MessageAliveTime => messageAliveTime;
    [SerializeField] private float messageAliveTime;

    public AnimationCurve ScaleCurve => scaleCurve;
    [SerializeField] private AnimationCurve scaleCurve;

    public void OnPlayerEmoted(EmoteMessage emoteMessage)
    {
        RectTransform rectTransform = transform as RectTransform;
        EmoteMessageDisplay messageDisplay = Instantiate(emoteMessagePrefab, transform);
        ((RectTransform)(messageDisplay.transform)).anchoredPosition 
            =  MathUtil.RandomVector2(-rectTransform.sizeDelta * 0.5f, rectTransform.sizeDelta * 0.5f);
        messageDisplay.Set(emoteMessage, this);
    }
}
