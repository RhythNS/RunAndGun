using MatchUp;
using Mirror;
using NobleConnect.Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Panel for connecting to another server.
/// </summary>
public class ServerConnect : PanelElement
{
    [SerializeField] private Button refreshButton;
    [SerializeField] private Button connectButton;
    [SerializeField] private Button abortConnectButton;
    [SerializeField] private Button reconnectButton;

    [SerializeField] private RectTransform contentTrans;
    [SerializeField] private RectTransform overviewTrans;
    [SerializeField] private RectTransform joinServerTrans;

    [SerializeField] private ServerConnectMatchDisplay matchDisplayPrefab;
    [SerializeField] private RectTransform noConnectionPrefab;

    [SerializeField] private TMP_Text nameDisplay;
    [SerializeField] private TMP_Text playerDisplay;
    [SerializeField] private TMP_Text regionDisplay;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private RectTransform passwordTrans;
    [SerializeField] private RectTransform matchInfoContentTrans;

    [SerializeField] private ConnectingScreen connectingScreen;

    [SerializeField] private float timeForDisconnectReconnect = 60.0f * 15.0f;
    [SerializeField] private float timeForConnectReconnect = 60.0f * 30.0f;

    private Match toConnectToMatch;

    private void Start()
    {
        refreshButton.onClick.AddListener(Refresh);
        connectButton.onClick.AddListener(OnConnectButtonPressed);
        abortConnectButton.onClick.AddListener(OnAbortConnectButtonPressed);
        reconnectButton.onClick.AddListener(OnReconnectButtonPressed);
    }

    public override void InnerOnShow()
    {
        joinServerTrans.gameObject.SetActive(false);
        overviewTrans.gameObject.SetActive(true);

        reconnectButton.gameObject.SetActive(ShouldShowReconnectButton());

        Refresh();
    }

    private bool ShouldShowReconnectButton()
    {
        LastConnectedGame lcg = Config.Instance.LastConnectedGame;
        if (lcg == null)
            return false;

        float reconnectTime = lcg.timeIsConnected ? timeForConnectReconnect : timeForDisconnectReconnect;
        return reconnectTime > lcg.GetSecondsTimeDifferenceSinceNow();
    }

    /// <summary>
    /// Refreshes the match list.
    /// </summary>
    private void Refresh()
    {
        refreshButton.gameObject.SetActive(false);

        if (RAGMatchmaker.Instance.IsReady == false)
        {
            new ExtendedCoroutine(
                this,
                RAGMatchmaker.Instance.Reconnect(),
                OnReconnected,
                true
            );
            return;
        }

        RAGMatchmaker.Instance.GetMatchList(OnMatchListRecieved);
    }

    /// <summary>
    /// Callback for when the matchmaker reconnected.
    /// </summary>
    private void OnReconnected()
    {
        if (RAGMatchmaker.Instance.IsReady == false)
        {
            DeleteAllContentChildren();
            Instantiate(noConnectionPrefab, contentTrans);
            refreshButton.gameObject.SetActive(true);
            return;
        }

        RAGMatchmaker.Instance.GetMatchList(OnMatchListRecieved);
    }

    /// <summary>
    /// Callback for when the match list was recieved.
    /// </summary>
    private void OnMatchListRecieved(bool success, Match[] matches)
    {
        refreshButton.gameObject.SetActive(true);

        DeleteAllContentChildren();

        if (success == false)
        {
            Instantiate(noConnectionPrefab, contentTrans);
            return;
        }

        Match currentMatch = RAGMatchmaker.Instance.GetCurrentMatch();
        ServerConnectMatchDisplay matchDisplay = Instantiate(matchDisplayPrefab);
        for (int i = 0; i < matches.Length; i++)
        {
            if (currentMatch != null && currentMatch == matches[i])
                continue;

            if (matchDisplay.Set(matches[i], this) == false)
                continue;

            matchDisplay.transform.SetParent(contentTrans);
            matchDisplay.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            matchDisplay = Instantiate(matchDisplayPrefab);
        }
        Destroy(matchDisplay.gameObject);
    }

    private void DeleteAllContentChildren()
    {
        while (contentTrans.childCount != 0)
        {
            Transform trans = contentTrans.GetChild(0);
            trans.SetParent(null);
            Destroy(trans.gameObject);
        }
    }

    /// <summary>
    /// When the connect button was pressed.
    /// </summary>
    private void OnConnectButtonPressed()
    {
        if (toConnectToMatch == null)
        {
            OnCancel();
            return;
        }

        RAGMatchmaker.Instance.JoinMatch(toConnectToMatch, OnMatchJoined);
    }

    /// <summary>
    /// When the abort button was pressed.
    /// </summary>
    private void OnAbortConnectButtonPressed()
    {
        joinServerTrans.gameObject.SetActive(false);
        overviewTrans.gameObject.SetActive(true);

        Refresh();
    }

    private void OnReconnectButtonPressed()
    {
        LastConnectedGame lcg = Config.Instance.LastConnectedGame;
        if (lcg == null)
            return;

        NetworkConnector.DisconnectClient();

        NobleNetworkManager manager = (NobleNetworkManager)NetworkManager.singleton;
        manager.networkAddress = lcg.lastConnectedIP;
        manager.networkPort = (ushort)lcg.lastConnectedPort;
        
        manager.StartClient();

        connectingScreen.Show();
        OnConfirm();
    }

    /// <summary>
    /// Callback for when a match was joined.
    /// </summary>
    private void OnMatchJoined(bool success, Match match)
    {
        if (success == false)
        {
            UIManager.Instance.ShowNotification("Could not connect to match");
            joinServerTrans.gameObject.SetActive(false);
            overviewTrans.gameObject.SetActive(true);
            return;
        }

        NetworkConnector.DisconnectClient();

        //NetworkManager.singleton.StartClient(match);
        NobleNetworkManager manager = (NobleNetworkManager)NetworkManager.singleton;
        string ip = match.matchData["ipAddress"];
        int port = match.matchData["port"];
        manager.networkAddress = ip;
        manager.networkPort = (ushort)port;

        Config.Instance.LastConnectedGame = LastConnectedGame.Connected(ip, port);

        manager.StartClient();

        connectingScreen.Show();
        OnConfirm();
    }

    /// <summary>
    /// Called when a match was clicked.
    /// </summary>
    /// <param name="match">The match to be connect to.</param>
    /// <param name="isPasswordProtected">Wheter the match is password protected.</param>
    /// <param name="matchName">The name of the match.</param>
    /// <param name="connected">How many players are currently connected.</param>
    /// <param name="maxPlayers">The maximum amount of players of the match.</param>
    /// <param name="regionString">The region of the match.</param>
    public void OnMatchClicked(Match match, bool isPasswordProtected, string matchName, int connected, int maxPlayers, string regionString)
    {
        joinServerTrans.gameObject.SetActive(true);
        overviewTrans.gameObject.SetActive(false);

        toConnectToMatch = match;

        nameDisplay.text = matchName;
        playerDisplay.text = connected + " / " + maxPlayers;
        regionDisplay.text = regionString;
        passwordInput.text = "";

        if (isPasswordProtected)
        {
            if (passwordTrans.parent == null)
                passwordTrans.parent = matchInfoContentTrans;
            passwordTrans.gameObject.SetActive(true);
        }
        else
        {
            if (passwordTrans.parent != null)
                passwordTrans.parent = null;
            passwordTrans.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        refreshButton.onClick.RemoveListener(Refresh);
        connectButton.onClick.RemoveListener(OnConnectButtonPressed);
    }
}
