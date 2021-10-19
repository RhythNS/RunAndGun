using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A element in the loading screen.
/// </summary>
public class LoadingPlayerElement : MonoBehaviour
{
    private LoadingScreenManager loadingScreenManager;

    [SerializeField] private Image background;
    [SerializeField] private Image unitIcon;
    [SerializeField] private TMP_Text username;
    [SerializeField] private PercentageAsText percentageAsText;
    [SerializeField] private PercentageAsImage percentageAsImage;
    [SerializeField] private bool fadeFromRight;

    private Player player;

    /// <summary>
    /// Sets all values.
    /// </summary>
    /// <param name="lsm">A reference to the loading screen manager.</param>
    /// <param name="player">The player which the loading element is representing. Can be null</param>
    /// <param name="backgroundColor">The background color of the element.</param>
    /// <param name="overrideFadeFromRight">True: force the panel to come from right, false: force the panel to come from left, null: do the expected entry.</param>
    public void Set(LoadingScreenManager lsm, Player player, Color backgroundColor, bool? overrideFadeFromRight = null)
    {
        loadingScreenManager = lsm;

        if (overrideFadeFromRight.HasValue)
            fadeFromRight = overrideFadeFromRight.Value;

        background.color = backgroundColor;

        if (player == null)
            return;

        this.player = player;
        username.text = player.entityName;
        unitIcon.sprite = CharacterDict.Instance.GetSpriteForType(player.CharacterType);

        if (lsm.Reconnecting == false)
        {
            player.StateCommunicator.OnPercentageChanged += percentageAsText.UpdateValue;
            player.StateCommunicator.OnPercentageChanged += percentageAsImage.UpdateValue;
        }
        else
        {
            percentageAsText.UpdateValue(1.0f);
            percentageAsImage.UpdateValue(1.0f);
        }
    }

    public void ForceUpdate(float percentage)
    {
        percentageAsText.UpdateValue(percentage);
        percentageAsImage.UpdateValue(percentage);
    }

    /// <summary>
    /// Starts the fading in of the element.
    /// </summary>
    public void FadeIn()
    {
        GetPositions(out Vector2 left, out Vector2 mid, out Vector2 right);
        transform.localPosition = fadeFromRight ? right : left;
        StartCoroutine(EnumeratorUtil.GoToInSecondsLocalCurve(transform, mid, loadingScreenManager.FadeInCurve, loadingScreenManager.FadeInTime));
    }

    /// <summary>
    /// Starts the fading out of the element.
    /// </summary>
    public void FadeOut()
    {
        GetPositions(out Vector2 left, out Vector2 mid, out Vector2 right);
        transform.localPosition = mid;
        StartCoroutine(EnumeratorUtil.GoToInSecondsLocalCurve(transform, fadeFromRight ? left : right, loadingScreenManager.FadeOutCurve, loadingScreenManager.FadeOutTime));
    }

    /// <summary>
    /// Sets all relevant positions.
    /// </summary>
    /// <param name="left">The left point of the loading screen.</param>
    /// <param name="mid">The mid point of the loading screen.</param>
    /// <param name="right">The right point of the loading screen.</param>
    public void GetPositions(out Vector2 left, out Vector2 mid, out Vector2 right)
    {
        float y = transform.localPosition.y;
        left = new Vector2(loadingScreenManager.Left, y);
        right = new Vector2(loadingScreenManager.Right, y);
        mid = new Vector2(loadingScreenManager.Mid, y);
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.StateCommunicator.OnPercentageChanged -= percentageAsText.UpdateValue;
            player.StateCommunicator.OnPercentageChanged -= percentageAsImage.UpdateValue;
        }
    }
}
