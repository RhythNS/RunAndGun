using MatchUp;
using Mirror;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ServerInfo : PanelElement
{
    [SerializeField] private TMP_InputField gamenameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private IntChooser maxPlayers;
    [SerializeField] private Button startServerButton;
    [SerializeField] private TMP_Text startServerText;
    [SerializeField] private Button stopServerButton;

    private void Start()
    {
        startServerButton.onClick.AddListener(StartServer);
        stopServerButton.onClick.AddListener(StopServer);
    }

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
        if (NetworkServer.active)
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

    private void StartServer()
    {
        int players = maxPlayers.Value;
        string matchName = gamenameInput.text;
        Config.Instance.password = passwordInput.text;

        Dictionary<string, MatchData> matchData = new Dictionary<string, MatchData>()
        {
            { "Match name", matchName },
            { "Max players", players },
            { "Password protected", (passwordInput.text.Length == 0 ? 0 : 1) }
        };

        if (RAGMatchmaker.Instance.GetCurrentMatch() != null)
        {
            RAGMatchmaker.Instance.SetMatchData(matchData);
            InnerOnConfirm();
            return;
        }

        RAGMatchmaker.Instance.HostMatch(matchData, OnMatchCreated);
        SetEnabled(false);
    }

    private void OnMatchCreated(bool success, Match match)
    {
        if (success == true)
        {
            OnConfirm();
            return;
        }

        SetEnabled(true);
        UIManager.Instance.ShowNotification("Could not create match! Check your internet connection!");
    }

    private void StopServer()
    {
        RAGMatchmaker.Instance.Disconnect();
        NetworkConnector.TryStartServer(true);
    }

    private void SetEnabled(bool enabled)
    {
        startServerButton.enabled = enabled;
        stopServerButton.enabled = enabled;
        cancelButton.enabled = enabled;
    }
}
