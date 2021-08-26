using UnityEngine;

/// <summary>
/// Dict for references to managing gameobjects.
/// </summary>
public class GlobalsDict : MonoBehaviour
{
    public static GlobalsDict Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("GlobalsDict already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// The gameobject on which all globals are on.
    /// </summary>
    public GameObject GlobalObject => globalObject;
    [SerializeField] private GameObject globalObject;

    /// <summary>
    /// The gameobject on which all dicts are on.
    /// </summary>
    public GameObject DictsObject => dictsObject;
    [SerializeField] private GameObject dictsObject;

    /// <summary>
    /// The gameobject on which all pools are on.
    /// </summary>
    public GameObject PoolObject => poolObject;
    [SerializeField] private GameObject poolObject;

    /// <summary>
    /// The gameobject on which all game state managers are on.
    /// </summary>
    public GameObject GameStateManagerObject => gameStateManagerObject;
    [SerializeField] private GameObject gameStateManagerObject;

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

}
