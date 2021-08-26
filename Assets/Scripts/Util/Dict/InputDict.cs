using UnityEngine;

/// <summary>
/// Dict for data used by input clases.
/// </summary>
public class InputDict : MonoBehaviour
{
    public static InputDict Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("InputDict already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// The custom mouse cursor.
    /// </summary>
    public Texture2D MouseCursor => mouseCursor;
    [SerializeField] private Texture2D mouseCursor;

    /// <summary>
    /// The prefab that should be spawned on mobule.
    /// </summary>
    public MobileUIManager MobileUIManagerPrefab => mobileUIManagerPrefab;
    [SerializeField] private MobileUIManager mobileUIManagerPrefab;

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
