using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI Element for managing the loading screen.
/// </summary>
public class LoadingScreenManager : MonoBehaviour
{
    public float Left { get; private set; } = 0.0f;
    public float Right { get; private set; } = 0.0f;
    public float Mid { get; private set; } = 0.0f;

    public bool Active { get; private set; } = false;

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

    public bool Reconnecting { get; private set; } = false;

    /// <summary>
    /// Shows the loading screen.
    /// </summary>
    public void Show(bool reconnecting = false)
    {
        Reconnecting = reconnecting;
        Active = true;
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
            // Check if there is a player connected in this slot. Then determine wheter the loading element
            // should come from the right or from the left, based on wheter the current index is an even number.
            if (player)
                CreateLoadingPlayerElement(i % 2 == 0 ? fromRightPrefab : fromLeftPrefab, -quaterHeight * i, player, CharacterDict.Instance.PlayerColors[i], width, height);
            else
                CreateLoadingPlayerElement(emptyPrefab, -quaterHeight * i, null, notConnectedColor, width, height, i % 2 == 0);
        }
        StartCoroutine(InnerShow());

        if (reconnecting)
            Player.LocalPlayer.StateCommunicator.OnPercentageChanged += OnReconnectPercentageChanged;
    }

    private void OnReconnectPercentageChanged(float newPerc)
    {
        if (newPerc < 0.99f)
            return;

        Player.LocalPlayer.StateCommunicator.OnPercentageChanged -= OnReconnectPercentageChanged;
        Hide();
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

    /// <summary>
    /// Creates a loading player element.
    /// </summary>
    /// <param name="prefab">The prefab to be used.</param>
    /// <param name="y">The y coordinate of where the element should be.</param>
    /// <param name="player">The player that the element is for. Can be null.</param>
    /// <param name="color">The color of the player.</param>
    /// <param name="width">The width of the loading screen.</param>
    /// <param name="height">The height of the loading screen.</param>
    /// <param name="overrideFadeFadeFromRight">True: force the panel to come from right, false: force the panel to come from left, null: do the expected entry.</param>
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

    /// <summary>
    /// Called to hide loading screen.
    /// </summary>
    public void Hide()
    {
        Active = false;
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
