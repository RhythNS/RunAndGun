using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenManager : MonoBehaviour
{
    public float Left { get; private set; } = 0.0f;
    public float Right { get; private set; } = 0.0f;
    public float Mid { get; private set; } = 0.0f;

    public float FadeInTime => fadeInTime;
    [SerializeField] private float fadeInTime;

    public AnimationCurve FadeInCurve => fadeInCurve;
    [SerializeField] private AnimationCurve fadeInCurve;

    public float FadeOutTime => fadeOutTime;
    [SerializeField] private float fadeOutTime;

    public AnimationCurve FadeOutCurve => fadeOutCurve;
    [SerializeField] private AnimationCurve fadeOutCurve;

    [SerializeField] private LoadingPlayerElement fromRightPrefab;
    [SerializeField] private LoadingPlayerElement fromLeftPrefab;
    [SerializeField] private LoadingPlayerElement emptyPrefab;

    [SerializeField] private Color notConnectedColor;

    [SerializeField] [EventRef] private string loadingStartSound;
    [SerializeField] [EventRef] private string loadingEndSound;

    private readonly List<LoadingPlayerElement> playerElements = new List<LoadingPlayerElement>();

    public void Show()
    {
        gameObject.SetActive(true);
        List<Player> players = PlayersDict.Instance.Players;

        RectTransform parentRect = (RectTransform)transform;
        float height = parentRect.rect.height;
        float quaterHeight = height * 0.25f;
        float width = parentRect.rect.width;

        Left = -width;
        Mid = 0.0f;
        Right = width;

        for (int i = 0; i < 4; i++)
        {
            Player player = players.Find(x => x.playerIndex == i);
            if (player)
                CreateLoadingPlayerElement(i % 2 == 0 ? fromRightPrefab : fromLeftPrefab, -quaterHeight * i, player, CharacterDict.Instance.PlayerColors[i], width, height);
            else
                CreateLoadingPlayerElement(emptyPrefab, -quaterHeight * i, null, notConnectedColor, width, height, i % 2 == 0);
        }
        /*
        for (int i = 0; i < players.Count; i++)
        {
            CreateLoadingPlayerElement(i % 2 == 0 ? fromRightPrefab : fromLeftPrefab, -quaterHeight * i, players[i], CharacterDict.Instance.PlayerColors[i], width, height);
        }
        for (int i = players.Count; i < 4; i++)
        {
            CreateLoadingPlayerElement(emptyPrefab, -quaterHeight * i, null, notConnectedColor, width, height, i % 2 == 0);
        }
         */
        StartCoroutine(InnerShow());
    }

    private IEnumerator InnerShow()
    {
        FMODUtil.PlayOneShot(loadingStartSound);
        for (int i = 0; i < playerElements.Count; i++)
        {
            playerElements[i].FadeIn();
            yield return new WaitForSeconds(fadeInTime * 0.25f);
        }
        yield return new WaitForSeconds(fadeInTime * 0.75f);
    }

    private void CreateLoadingPlayerElement(LoadingPlayerElement prefab, float y, Player player, Color color, float width, float height, bool? overrideFadeFadeFromRight = null)
    {
        LoadingPlayerElement lpe = Instantiate(prefab, transform);
        RectTransform rectTrans = (RectTransform)lpe.transform;
        //rectTrans.sizeDelta = new Vector2(width, height * 0.25f);
        //rectTrans.position = new Vector3(0.0f, y);
        rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height * 0.25f);
        rectTrans.localPosition = new Vector3(width, y);
        lpe.Set(this, player, color, overrideFadeFadeFromRight);
        playerElements.Add(lpe);
    }

    public void Hide()
    {
        StartCoroutine(InnerHide());
    }

    private IEnumerator InnerHide()
    {
        FMODUtil.PlayOneShot(loadingEndSound);
        for (int i = 0; i < playerElements.Count; i++)
        {
            playerElements[i].FadeOut();
            yield return new WaitForSeconds(fadeOutTime * 0.25f);
        }
        yield return new WaitForSeconds(fadeOutTime * 0.75f);

        for (int i = 0; i < playerElements.Count; i++)
        {
            Destroy(playerElements[i].gameObject);
        }
        playerElements.Clear();
        gameObject.SetActive(false);
    }
}
