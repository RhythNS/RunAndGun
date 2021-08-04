using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    }

    public void Set(Dictionary<Player, Dictionary<Type, Stat>> stats)
    {
        StartCoroutine(InnerSet(stats));
    }

    private IEnumerator InnerSet(Dictionary<Player, Dictionary<Type, Stat>> stats)
    {
        yield return new WaitForSeconds(showAfterSeconds);

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

        foreach (Player player in stats.Keys)
        {
            Dictionary<Type, Stat> dict = stats[player];

            if (dict.Count == 0)
                continue;

            GameOverPlayer gop = Instantiate(playerPrefab, contentTrans);
            gop.Set(player, dict);
        }
    }

    private void OnLeaveServerClicked()
    {

    }

    private void OnReturnToLobbyClicked()
    {
        GameManager.BackToLobby();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

}
