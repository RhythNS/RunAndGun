using MatchUp;
using Mirror;
using NobleConnect.Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ServerConnect : PanelElement
{
    [SerializeField] private Button refreshButton;
    [SerializeField] private Button connectButton;

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

    private Match toConnectToMatch;

    private void Start()
    {
        refreshButton.onClick.AddListener(Refresh);
        connectButton.onClick.AddListener(OnConnectButtonPressed);
    }

    public override void InnerOnShow()
    {
        joinServerTrans.gameObject.SetActive(false);
        overviewTrans.gameObject.SetActive(true);

        Refresh();
    }

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

    private void OnReconnected()
    {
        RAGMatchmaker.Instance.GetMatchList(OnMatchListRecieved);
    }

    private void OnMatchListRecieved(bool success, Match[] matches)
    {
        refreshButton.gameObject.SetActive(true);

        while (contentTrans.childCount != 0)
        {
            Transform trans = contentTrans.GetChild(0);
            trans.parent = null;
            Destroy(trans.gameObject);
        }

        if (success == false)
        {
            Instantiate(noConnectionPrefab, contentTrans);
            return;
        }

        ServerConnectMatchDisplay matchDisplay = Instantiate(matchDisplayPrefab);
        for (int i = 0; i < matches.Length; i++)
        {
            if (matchDisplay.Set(matches[i], this) == false)
                continue;

            matchDisplay.transform.SetParent(contentTrans);
            matchDisplay = Instantiate(matchDisplayPrefab);
        }
        Destroy(matchDisplay);
    }

    private void OnConnectButtonPressed()
    {
        if (toConnectToMatch == null)
        {
            OnCancel();
            return;
        }

        RAGMatchmaker.Instance.JoinMatch(toConnectToMatch, OnMatchJoined);
    }

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
        manager.StartClient();

        connectingScreen.Show();
        OnConfirm();
    }

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
