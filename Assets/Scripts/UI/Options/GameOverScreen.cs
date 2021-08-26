using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI element manager for showing the game over screen.
/// </summary>
public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button leaveServerButton;
    [SerializeField] private RectTransform contentTrans;
    [SerializeField] private GameOverPlayer playerPrefab;
    [SerializeField] private TMP_Text waitingForHostText;

    [SerializeField] private float showAfterSeconds = 2.0f;

    public static GameOverScreen Instance { get; private set; }

    public GameOverStat statPrefab;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("GameOverScreen already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;

        backButton.onClick.AddListener(OnReturnToLobbyClicked);
        leaveServerButton.onClick.AddListener(OnLeaveServerClicked);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets all stats of all players.
    /// </summary>
    public void Set(StatsTransmission stats)
    {
        GlobalsDict.Instance.StartCoroutine(InnerSet(stats));
    }

    private IEnumerator InnerSet(StatsTransmission stats)
    {
        yield return new WaitForSeconds(showAfterSeconds);
        gameObject.SetActive(true);

        if (Player.LocalPlayer.isServer)
        {
            waitingForHostText.gameObject.SetActive(false);
            backButton.gameObject.SetActive(true);
            leaveServerButton.gameObject.SetActive(false);
        }
        else
        {
            waitingForHostText.gameObject.SetActive(true);
            backButton.gameObject.SetActive(false);
            leaveServerButton.gameObject.SetActive(true);
        }

        while (contentTrans.childCount > 0)
        {
            Transform trans = contentTrans.GetChild(0);
            trans.SetParent(null);
            Destroy(trans.gameObject);
        }

        for (int i = 0; i < stats.stats.Length; i++)
        {
            GameOverPlayer gop = Instantiate(playerPrefab, contentTrans);
            gop.Set(stats.names[i], stats.characterTypes[i], stats.stats[i]);
        }
    }

    /// <summary>
    /// Called when a connected client wants to leave the server.
    /// </summary>
    private void OnLeaveServerClicked()
    {
        gameObject.SetActive(false);
        StartCoroutine(RegionSceneLoader.Instance.LoadScene(Region.Lobby));
    }

    /// <summary>
    /// Called when a host wants to retun to the lobby.
    /// </summary>
    private void OnReturnToLobbyClicked()
    {
        gameObject.SetActive(false);
        GameManager.BackToLobby();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
