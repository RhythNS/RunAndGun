using UnityEngine;

public class LobbyLevel : MonoBehaviour
{
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

    public void Show()
    {
        gameObject.SetActive(true);
        MusicManager.Instance.ChangeState(MusicManager.State.Lobby);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
