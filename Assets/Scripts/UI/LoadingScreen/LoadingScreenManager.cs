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

    private readonly List<LoadingPlayerElement> playerElements = new List<LoadingPlayerElement>();

    public void Show()
    {
        List<Player> players = PlayersDict.Instance.Players;
        float height = Screen.height;
        float width = Screen.width;

        Left = -width;
        Mid = 0.0f;
        Right = width;

        for (int i = 0; i < players.Count; i++)
        {
            CreateLoadingPlayerElement(i % 2 == 0 ? fromRightPrefab : fromLeftPrefab, height / (i + 1), players[i], width, height);
        }
        for (int i = players.Count; i < 4; i++)
        {
            CreateLoadingPlayerElement(emptyPrefab, height / (i + 1), null, width, height);
        }
        StartCoroutine(InnerShow());
    }

    private IEnumerator InnerShow()
    {
        for (int i = 0; i < playerElements.Count; i++)
        {
            playerElements[i].FadeIn();
            yield return new WaitForSeconds(fadeInTime * 0.25f);
        }
        yield return new WaitForSeconds(fadeInTime * 0.75f);
    }

    private void CreateLoadingPlayerElement(LoadingPlayerElement prefab, float y, Player player, float width, float height)
    {
        LoadingPlayerElement lpe = Instantiate(prefab, transform);
        RectTransform rectTrans = (RectTransform)lpe.transform;
        rectTrans.sizeDelta = new Vector2(width, height * 0.25f);
        rectTrans.position = new Vector3(0.0f, y);
        lpe.Set(this, player);
        playerElements.Add(lpe);
    }

    public void Hide()
    {
        StartCoroutine(InnerHide());
    }

    private IEnumerator InnerHide()
    {
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
    }
}
