using UnityEngine;

public class UIManager : MonoBehaviour
{
    public Canvas Canvas => canvas;
    [SerializeField] Canvas canvas;

    [SerializeField] private LoadingScreenManager loadingScreenManager;
    [SerializeField] private InGameManager inGameManager;

    public OptionsUIManager OptionsManager => optionsManager;
    [SerializeField] private OptionsUIManager optionsManager;

    public static UIManager Instance { get; private set; }
    private MobileUIManager mobileUiManager;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("UIManager already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
        inGameManager.gameObject.SetActive(false);
        loadingScreenManager.gameObject.SetActive(false);
    }

    public void OnLocalPlayerStarted(Player player, InputType inputMethod)
    {
        if (inputMethod == InputType.Mobile)
        {
            mobileUiManager = Instantiate(InputDict.Instance.MobileUIManagerPrefab, canvas.transform);
            (player.Input as MobileInput).SetUI(mobileUiManager);
        }
        inGameManager.RegisterEvents(player);
    }

    public void OnLocalPlayerDeleted() => inGameManager.UnRegisterEvents();

    public void ShowLevelLoadScreen() => loadingScreenManager.Show();

    public void HideLevelLoadScreen() => loadingScreenManager.Hide();

    public void OnPlayerEmoted(EmoteMessage emoteMessage) => inGameManager.OnPlayerEmoted(emoteMessage);

    public void ToggleEmotePanel() => inGameManager.ToggleEmotePanel();

    public void ShowNotification(string toDisplay) => inGameManager.ShowNotification(toDisplay);

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

}
