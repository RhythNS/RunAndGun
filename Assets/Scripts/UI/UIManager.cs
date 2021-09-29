﻿using UnityEngine;

/// <summary>
/// Holds information about ui managers.
/// </summary>
public class UIManager : MonoBehaviour
{
    /// <summary>
    /// The root canvas of the ui.
    /// </summary>
    public Canvas Canvas => canvas;
    [SerializeField] Canvas canvas;

    [SerializeField] private LoadingScreenManager loadingScreenManager;
    [SerializeField] private InGameManager inGameManager;

    /// <summary>
    /// The manager that manages the options.
    /// </summary>
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
            if (!mobileUiManager)
                mobileUiManager = Instantiate(InputDict.Instance.MobileUIManagerPrefab, canvas.transform);
            (player.Input as MobileInput).SetUI(mobileUiManager);
        }
        inGameManager.RegisterEvents(player);
    }

    public void OnLocalPlayerDeleted() => inGameManager.UnRegisterEvents();

    public void ShowLevelLoadScreen(bool reconnecting = false) => loadingScreenManager.Show(reconnecting);
    public void HideLevelLoadScreen() => loadingScreenManager.Hide();
    public bool IsLoadingScreenActive() => loadingScreenManager.Active;

    public void OnPlayerEmoted(EmoteMessage emoteMessage) => inGameManager.OnPlayerEmoted(emoteMessage);

    public void ToggleEmotePanel() => inGameManager.ToggleEmotePanel();

    public void ShowNotification(string toDisplay) => optionsManager.ShowNotification(toDisplay);

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

}
