using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI Element that displays when a client wants to connect to a server.
/// </summary>
public class ConnectingScreen : MonoBehaviour
{
    [SerializeField] private Image walkingSprite;
    [SerializeField] private Sprite[] walkingAnimation;
    [SerializeField] private float timePerSprite;

    private float timer;
    private int atSprite;

    public void Show()
    {
        gameObject.SetActive(true);
        StartCoroutine(CheckForConnected());
    }

    /// <summary>
    /// Checks if the player is connected to the server.
    /// </summary>
    private IEnumerator CheckForConnected()
    {
        NetworkManager netManager = NetworkManager.singleton;

        while (true)
        {
            switch (netManager.mode)
            {
                case NetworkManagerMode.Offline:
                case NetworkManagerMode.Host:
                case NetworkManagerMode.ServerOnly:
                    NetworkConnector.TryStartServer(false);
                    yield return null;
                    UIManager.Instance.ShowNotification("Could not connect to server!");
                    gameObject.SetActive(false);
                    yield break;

                case NetworkManagerMode.ClientOnly:
                    if (NetworkClient.isConnected == true)
                    {
                        gameObject.SetActive(false);
                        yield break;
                    }
                    break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0.0f)
        {
            timer = timePerSprite;
            if (++atSprite >= walkingAnimation.Length)
                atSprite = 0;
            walkingSprite.sprite = walkingAnimation[atSprite];
        }
    }
}
