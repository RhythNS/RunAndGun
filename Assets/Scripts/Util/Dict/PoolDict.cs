using UnityEngine;

/// <summary>
/// Dict for all Network pools.
/// </summary>
public class PoolDict : MonoBehaviour
{
    public static PoolDict Instance { get; private set; }

    private void Awake()
    {
        if (Instance == this)
        {
            Debug.LogWarning("Already a PoolDict in scene! Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Pool for bullets.
    /// </summary>
    public NetworkPool BulletPool => bulletPool;
    [SerializeField] private NetworkPool bulletPool;

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
