using Mirror;
using NobleConnect.Mirror;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ServerInfo : PanelElement
{
    [SerializeField] private TMP_InputField ipInput;
    [SerializeField] private TMP_InputField portInput;
    [SerializeField] private Button startServerButton;

   // private ExtendedCoroutine waitingForConnection;

    public override void InnerOnShow()
    {
        RAGNetworkManager networkManager = (RAGNetworkManager)NetworkManager.singleton;

        if (networkManager.HostEndPoint == null)
        {
            startServerButton.gameObject.SetActive(false);
            ipInput.gameObject.SetActive(true);
            portInput.gameObject.SetActive(true);

            ipInput.text = "No internet connection";
            portInput.text = "";

            return;
        }

        if (networkManager.IsLanOnly == true)
        {
            startServerButton.gameObject.SetActive(true);
            ipInput.gameObject.SetActive(false);
            portInput.gameObject.SetActive(false);

            return;
        }

        startServerButton.gameObject.SetActive(false);
        ipInput.gameObject.SetActive(true);
        portInput.gameObject.SetActive(true);

        ipInput.text = networkManager.HostEndPoint.Address.ToString();
        portInput.text = networkManager.HostEndPoint.Port.ToString();
    }
    /*
    public override void InnerOnHide()
    {
        base.InnerOnHide();

        if (waitingForConnection == null || waitingForConnection.IsFinshed == true)
            return;

        waitingForConnection.Stop(false);
    }
     */

    public void StartServer()
    {
        NetworkConnector.TryStartServer(false);

        /*
        RAGNetworkManager networkManager = (RAGNetworkManager)NetworkManager.singleton;

        networkManager.StopHost();
        networkManager.StartHost();

        waitingForConnection = new ExtendedCoroutine(this, WaitForConnection(), InnerOnShow, true);
         */
    }
    /*
    private IEnumerator WaitForConnection()
    {
        RAGNetworkManager networkManager = (RAGNetworkManager)NetworkManager.singleton;

        float timeout = 5.0f;

        while (timeout > 0.0f)
        {
            if (networkManager.isNetworkActive && networkManager.client != null && networkManager.client.isConnected)
                yield break;

            yield return new WaitForSeconds(0.5f);
            timeout -= 0.5f;
        }

        networkManager.StartHostLANOnly();
    }
     */
}
