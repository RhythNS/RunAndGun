using UnityEngine;

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

    public GameObject GlobalObject => globalObject;
    [SerializeField] private GameObject globalObject;

    public GameObject DictsObject => dictsObject;
    [SerializeField] private GameObject dictsObject;

    public GameObject PoolObject => poolObject;
    [SerializeField] private GameObject poolObject;

    public GameObject GameStateManagerObject => gameStateManagerObject;
    [SerializeField] private GameObject gameStateManagerObject;

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

}
