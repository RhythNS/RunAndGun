using UnityEngine;

/// <summary>
/// Holds all config values.
/// </summary>
public class Config : MonoBehaviour
{
    // Singleton
    public static Config Instance { get; private set; }

    private void Awake()
    {
        if (Instance) // Am I already set and in the scene?
        {
            Debug.LogWarning("Config already in scene! Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

#if !UNITY_EDITOR
    fmodAudioDebugOverlayEnabled = false;
#endif

        LoadValues();
    }

    /// <summary>
    /// Loads all config values from config file.
    /// </summary>
    private void LoadValues()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.LinuxPlayer:
                
                targetFramesPerSecondLoadingScreen = 60;
                break;

            case RuntimePlatform.IPhonePlayer:
            case RuntimePlatform.Android:
                
                targetFramesPerSecondLoadingScreen = 30;
                break;

            default:
                break;
        }
        // TODO: load some stuff
    }

    // ---- Connection ----
    public string playerName = "Test";
    public CharacterType selectedPlayerType = CharacterType.Melee;

    // ---- Input ----
    public InputType selectedInput = InputType.KeyMouse;
    public bool useFocusPoint = true;

    // Graphics
    public int targetFramesPerSecondLoadingScreen = 60;

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
