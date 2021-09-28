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

    private ExtendedCoroutine checkForConnect, waitAndConnect;

    private float timer;
    private int atSprite;

    public void Show()
    {
        gameObject.SetActive(true);
        checkForConnect = new ExtendedCoroutine(this, CheckForConnected(), startNow: true);
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
                    OnFailed();
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

    public void OnFailed()
    {
        if (checkForConnect != null && checkForConnect.IsFinshed == false)
            checkForConnect.Stop(false);

        if (waitAndConnect == null || waitAndConnect.IsFinshed == true)
            waitAndConnect = new ExtendedCoroutine(this, WaitAndConnect(), startNow: true);
    }

    private IEnumerator WaitAndConnect()
    {
        NetworkConnector.TryStartServer(false);
        yield return null;
        UIManager.Instance.ShowNotification("Could not connect to server!");
        gameObject.SetActive(false);
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
