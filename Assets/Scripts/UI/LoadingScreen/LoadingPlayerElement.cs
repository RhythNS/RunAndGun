using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPlayerElement : MonoBehaviour
{
    private LoadingScreenManager loadingScreenManager;

    [SerializeField] private Image unitIcon;
    [SerializeField] private TMP_Text username;
    [SerializeField] private PercentageAsText percentageAsText;
    [SerializeField] private bool fadeFromRight;

    private Player player;

    public void Set(LoadingScreenManager lsm, Player player)
    {
        loadingScreenManager = lsm;

        if (player == null)
            return;

        this.player = player;
        username.text = player.userName;
        unitIcon.sprite = CharacterDict.Instance.GetSpriteForType(player.CharacterType);
        player.StateCommunicator.OnPercentageChanged += percentageAsText.UpdateValue;
    }

    public void FadeIn()
    {
        GetPositions(out Vector2 left, out Vector2 mid, out Vector2 right);
        transform.position = fadeFromRight ? right : left;
        StartCoroutine(EnumeratorUtil.GoToInSecondsCurve(transform, mid, loadingScreenManager.FadeInCurve, loadingScreenManager.FadeInTime));
    }

    public void FadeOut()
    {
        GetPositions(out Vector2 left, out Vector2 mid, out Vector2 right);
        transform.position = mid;
        StartCoroutine(EnumeratorUtil.GoToInSecondsCurve(transform, fadeFromRight ? left : right, loadingScreenManager.FadeOutCurve, loadingScreenManager.FadeOutTime));
    }

    public void GetPositions(out Vector2 left, out Vector2 mid, out Vector2 right)
    {
        float y = transform.position.y;
        left = new Vector2(loadingScreenManager.Left, y);
        right = new Vector2(loadingScreenManager.Right, y);
        mid = new Vector2(loadingScreenManager.Mid, y);
    }

    private void OnDestroy()
    {
        if (player != null)
            player.StateCommunicator.OnPercentageChanged -= percentageAsText.UpdateValue;
    }
}
