using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI Element for sending emotes.
/// </summary>
public class EmoteManager : MonoBehaviour
{
    [SerializeField] private Button showManagerButton;
    [SerializeField] private List<EmoteButton> emoteButtons;
    [SerializeField] private List<EmoteChangeButton> emoteChangeButtons;
    [SerializeField] private RectTransform typeParent;
    [SerializeField] private RectTransform emoteButtonParent;
    [SerializeField] private EmoteChangeButton emoteChangeButtonPrefab;
    [SerializeField] private AnimationCurve hideShowCurve;
    [SerializeField] private float showInSeconds;

    public bool Hidden { get; private set; }

    private ExtendedCoroutine extendedCoroutine;
    private Vector2 endPos;

    private void Awake()
    {
        for (int i = 0; i < emoteButtons.Count; i++)
            emoteButtons[i].manager = this;
    }

    private void Start()
    {
        EmoteDict.EmoteLayer[] emoteLayers = EmoteDict.Instance.EmoteLayers;

        for (int i = 0; i < emoteLayers.Length; i++)
        {
            EmoteChangeButton emoteChangeButton = Instantiate(emoteChangeButtonPrefab, typeParent);
            emoteChangeButton.Set(this, emoteLayers[i].name, i);
            emoteChangeButtons.Add(emoteChangeButton);
        }

        if (emoteLayers.Length > 0)
            OnLayerChange(0);

        showManagerButton.gameObject.SetActive(Player.LocalPlayer.Input is MobileInput);

        endPos = transform.localPosition;
        transform.localPosition = GetStartPos();

        Hidden = true;
    }

    /// <summary>
    /// Get the starting position when entering or the end position when hiding.
    /// </summary>
    private Vector2 GetStartPos()
    {
        RectTransform ownTrans = transform as RectTransform;

        return endPos - ownTrans.rect.width * Vector2.right;
    }

    /// <summary>
    /// Show the element.
    /// </summary>
    public void Show()
    {
        if (extendedCoroutine != null && extendedCoroutine.IsFinshed == false)
            return;

        extendedCoroutine = new ExtendedCoroutine
            (
                this,
                EnumeratorUtil.GoToInSecondsLocalCurve(transform, endPos, hideShowCurve, showInSeconds),
                OnShownFinished,
                true
            );
    }

    /// <summary>
    /// Called when shown finished.
    /// </summary>
    public void OnShownFinished()
    {
        Hidden = false;
    }

    /// <summary>
    /// Called when the current layer should change.
    /// </summary>
    /// <param name="layerId">The new emote layer.</param>
    public void OnLayerChange(int layerId)
    {
        EmoteDict.EmoteLayer layer = EmoteDict.Instance.EmoteLayers[layerId];

        if (emoteButtons.Count < layer.emotes.Length)
            Debug.LogError("There are more emotes in layer " + layer.name + " then there are buttons!");

        int maxLength = Mathf.Min(emoteButtons.Count, layer.emotes.Length);
        for (int i = 0; i < maxLength; i++)
        {
            emoteButtons[i].gameObject.SetActive(true);
            emoteButtons[i].Set(layer.emotes[i]);
        }
        for (int i = maxLength; i < emoteButtons.Count; i++)
        {
            emoteButtons[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Called when an emote was clicked.
    /// </summary>
    /// <param name="emoteID">The id of the emote.</param>
    public void OnEmote(int emoteID)
    {
        if (extendedCoroutine != null && extendedCoroutine.IsFinshed == false)
            return;

        if (Player.LocalPlayer)
            Player.LocalPlayer.EmoteCommunicator.CmdSend(emoteID);

        Hide();
    }

    /// <summary>
    /// Hides the element.
    /// </summary>
    public void Hide()
    {
        if (extendedCoroutine != null && extendedCoroutine.IsFinshed == false)
            return;

        extendedCoroutine = new ExtendedCoroutine(this, OnHide(), OnHideFinished, true);
    }

    public IEnumerator OnHide()
    {
        yield return EnumeratorUtil.GoToInSecondsLocalCurve(transform, GetStartPos(), hideShowCurve, showInSeconds);
    }

    /// <summary>
    /// Called when the element is fully hidden.
    /// </summary>
    private void OnHideFinished()
    {
        Hidden = true;
    }
}
