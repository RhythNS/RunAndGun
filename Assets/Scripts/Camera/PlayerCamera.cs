using UnityEngine;

/// <summary>
/// Camera control for Players. A focus point can be set to slightly adjust
/// the position of the camera.
/// </summary>
public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private float focusPointStrength = 0.25f;
    [SerializeField] private float z;
    [SerializeField] private Transform toFollow;

    public static PlayerCamera Instance { get; private set; }

    /// <summary>
    /// Should be the player that the camera follows.
    /// </summary>
    public Transform ToFollow
    {
        get => toFollow;
        set
        {
            enabled = value != null;
            toFollow = value;
        }
    }
    
    /// <summary>
    /// Focus point to slightly adjust the position of the camera.
    /// </summary>
    public Vector2 focusPoint = new Vector2(0.0f, 0.0f);

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("PlayerCamera already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        Vector2 pos = ToFollow.position;
        Vector2 dir = focusPoint - pos;
        pos += dir * focusPointStrength;

        transform.position = new Vector3(pos.x, pos.y, z);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
