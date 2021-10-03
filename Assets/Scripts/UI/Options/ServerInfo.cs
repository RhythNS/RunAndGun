using MatchUp;
using Mirror;
using NobleConnect.Mirror;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Panel for editing and showing information about the server.
/// </summary>
public class ServerInfo : PanelElement
{
    [SerializeField] private TMP_InputField gamenameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private IntChooser maxPlayers;
    [SerializeField] private Button startServerButton;
    [SerializeField] private TMP_Text startServerText;
    [SerializeField] private Button stopServerButton;
    [SerializeField] private TMP_Dropdown regionDropDown;

    private int players;
    private Dictionary<string, MatchData> matchData;

    private void Start()
    {
        startServerButton.onClick.AddListener(StartServer);
        stopServerButton.onClick.AddListener(StopServer);
    }

    public override bool ShouldShow() => NetworkServer.active == true;

    public override void InnerOnShow()
    {
        SetEnabled(true);

        Match currentMatch = RAGMatchmaker.Instance.GetCurrentMatch();

        if (currentMatch == null)
        {
            startServerText.text = "Start Server";
            stopServerButton.gameObject.SetActive(false);
            return;
        }
        else if (NetworkServer.active)
        {
            stopServerButton.gameObject.SetActive(true);
            startServerText.text = "Update Server";
            return;
        }
        else
        {
            OnCancel();
            return;
        }
    }

    /// <summary>
    /// Start the server based on current inputs.
    /// </summary>
    private void StartServer()
    {
        players = maxPlayers.Value;
        string matchName = gamenameInput.text;
        Config.Instance.password = passwordInput.text;
        int regionVal = regionDropDown.value + 1;

        matchData = new Dictionary<string, MatchData>()
        {
            { "Match name", matchName },
            { "Max players", players },
            { "Connected players", 1 },
            { "Region", regionVal },
            { "Password protected", (passwordInput.text.Length == 0 ? 0 : 1) }
        };

        if (RAGMatchmaker.Instance.GetCurrentMatch() != null)
        {
            RAGMatchmaker.Instance.SetMatchData(matchData);
            OnConfirm();
            return;
        }

        GlobalsDict.Instance.StartCoroutine(NetworkConnector.RestartServerWithInternetConnection(OnServerStarted, OnServerFailedToConnect));
        SetEnabled(false);
    }

    private void OnServerStarted()
    {
        RAGMatchmaker.Instance.HostMatch(matchData, players, OnMatchCreated);
    }

    private void OnServerFailedToConnect()
    {
        UIManager.Instance.ShowNotification("Could not host server! Check your internet connection!");
    }

    /// <summary>
    /// Callback for when a match was created.
    /// </summary>
    private void OnMatchCreated(bool success, Match match)
    {
        Debug.Log("Match hosted: " + success);

        if (success == true)
        {
            OnConfirm();
            return;
        }

        SetEnabled(true);
        UIManager.Instance.ShowNotification("Could not create match! Check your internet connection!");
    }

    /// <summary>
    /// Stops the server.
    /// </summary>
    private void StopServer()
    {
        RAGMatchmaker.Instance.Disconnect();
        OnCancel();
    }

    /// <summary>
    /// Dis- or enables all buttons.
    /// </summary>
    private void SetEnabled(bool enabled)
    {
        startServerButton.enabled = enabled;
        stopServerButton.enabled = enabled;
        cancelButton.enabled = enabled;
    }
}
