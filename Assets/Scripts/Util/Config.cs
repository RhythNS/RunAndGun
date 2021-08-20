using UnityEngine;

/// <summary>
/// Holds all config values.
/// </summary>
public class Config : MonoBehaviour
{
    [SerializeField] private bool ignorePlatform = false;

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

        LoadValues();
    }

    /// <summary>
    /// Loads all config values from config file.
    /// </summary>
    private void LoadValues()
    {
        if (ignorePlatform == false)
            SetValuesForPlattform();

        SaveGame loaded = Saver.Load();
        if (loaded == null)
            return;

        saveFileExisted = true;
        playerName = loaded.playerName;
        selectedPlayerType = loaded.lastSelectedCharacterType;
    }

    private void SetValuesForPlattform()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.LinuxEditor:
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.LinuxPlayer:

                targetFramesPerSecondLoadingScreen = 60;
                selectedInput = InputType.KeyMouse;
                return;

            case RuntimePlatform.IPhonePlayer:
            case RuntimePlatform.Android:

                targetFramesPerSecondLoadingScreen = 30;
                selectedInput = InputType.Mobile;
                return;
        }
    }

    public void Save()
    {
        SaveGame saveGame = new SaveGame()
        {
            playerName = PlayerName,
            lastSelectedCharacterType = SelectedPlayerType
        };
        Saver.Save(saveGame);
    }

    // ---- Config ----
    public static bool saveFileExisted = false;

    // ---- Connection ----
    private string playerName = "Test";
    public string password = "";
    private CharacterType selectedPlayerType = CharacterType.Melee;

    public string PlayerName
    {
        get => playerName; set
        {
            playerName = value;
            Save();
        }
    }
    public CharacterType SelectedPlayerType
    {
        get => selectedPlayerType; set
        {
            selectedPlayerType = value;
            Save();
        }
    }


    // ---- Input ----
    public InputType selectedInput = InputType.KeyMouse;
    public bool useFocusPoint = true;

    // ---- Graphics ----
    public int targetFramesPerSecondLoadingScreen = 60;

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
