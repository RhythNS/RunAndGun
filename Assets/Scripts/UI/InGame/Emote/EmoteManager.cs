using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoteManager : MonoBehaviour
{
    [SerializeField] private List<EmoteButton> emoteButtons;
    [SerializeField] private List<EmoteChangeButton> emoteChangeButtons;
    [SerializeField] private RectTransform typeParent;
    [SerializeField] private RectTransform emoteButtonParent;
    [SerializeField] private EmoteChangeButton emoteChangeButtonPrefab;
    [SerializeField] private AnimationCurve hideShowCurve;
    [SerializeField] private float showInSeconds;

    private ExtendedCoroutine extendedCoroutine;
    private Vector2 endPos;

    private void Awake()
    {
        for (int i = 0; i < emoteButtons.Count; i++)
            emoteButtons[i].manager = this;

        endPos = transform.position;
        transform.position = GetStartPos();
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

        gameObject.SetActive(false);
    }

    private Vector2 GetStartPos()
    {
        RectTransform ownTrans = transform as RectTransform;
        Vector2 retPos = ownTrans.position;
        retPos.x = -ownTrans.sizeDelta.x;
        return retPos;
    }

    public void Show()
    {
        if (extendedCoroutine != null && extendedCoroutine.IsFinshed == false)
            return;

        gameObject.SetActive(true);

        extendedCoroutine = new ExtendedCoroutine
            (
                this,
                EnumeratorUtil.GoToInSecondsCurve(transform, endPos, hideShowCurve, showInSeconds),
                startNow: true
            );
    }

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

    public void OnEmote(int emoteID)
    {
        if (extendedCoroutine != null && extendedCoroutine.IsFinshed == false)
            return;

        if (Player.LocalPlayer)
            Player.LocalPlayer.EmoteCommunicator.CmdSend(emoteID);

        Hide();
    }

    public void Hide()
    {
        if (extendedCoroutine != null && extendedCoroutine.IsFinshed == false)
            return;

        extendedCoroutine = new ExtendedCoroutine(this, OnHide(), startNow: true);
    }

    public IEnumerator OnHide()
    {
        yield return EnumeratorUtil.GoToInSecondsCurve(transform, GetStartPos(), hideShowCurve, showInSeconds);
        gameObject.SetActive(false);
    }
}
