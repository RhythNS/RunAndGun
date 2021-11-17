using UnityEngine;

/// <summary>
/// Manages the lobby level.
/// </summary>
public class LobbyLevel : MonoBehaviour
{
    [System.Serializable]
    private class Bounds
    {
        public Vector2 position;
        public Vector2 scale;
    }

    [SerializeField] private Bounds bounds;
    [SerializeField] private GameObject lobbyPathfinder;

    public static LobbyLevel Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("LobbyLevel already in scene! Deleting this gameobject!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Shows the level.
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
        StartCoroutine(DungeonCreator.Instance.ClearPreviousDungeon());
        MusicManager.Instance.ChangeState(MusicManager.State.Lobby);
        DungeonCreator.Instance.AdjustMask(bounds.position, bounds.scale);
        IPathfinder pathfinder = lobbyPathfinder.GetComponent<IPathfinder>();
        if (pathfinder == null)
            Debug.LogWarning("Could not get a pathfinder from the pathfinder object!");
        DungeonDict.Instance.pathfinder = pathfinder;
        ConversionDict.Instance.offsetPosition = Vector3.zero;
    }

    /// <summary>
    /// Hides the level.
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bounds.position + (bounds.scale * 0.5f), bounds.scale);
    }
}
