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

        LoadValues();
    }

    /// <summary>
    /// Loads all config values from config file.
    /// </summary>
    private void LoadValues()
    {
        // TODO: load some stuff
    }

    public InputType selectedInput = InputType.KeyMouse;

    private void OnDestroy()
    {
        Instance = null;
    }
}
