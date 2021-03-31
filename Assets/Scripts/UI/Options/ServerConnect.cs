using Mirror;
using NobleConnect.Mirror;
using System.Collections;
using TMPro;
using UnityEngine;

public class ServerConnect : PanelElement
{
    [SerializeField] private TMP_InputField ipInput;
    [SerializeField] private TMP_InputField portInput;

    public override bool InnerOnConfirm()
    {
        NobleNetworkManager networkManager = (NobleNetworkManager)NetworkManager.singleton;

        if (ushort.TryParse(portInput.text, out ushort port) == false)
            return false;

        Config.Instance.StartCoroutine(StartConnect(networkManager, ipInput.text, port));

        return true;
    }

    private IEnumerator StartConnect(NobleNetworkManager networkManager, string networkAdress, ushort port)
    {
        networkManager.StopHost();

        yield return new WaitForSeconds(1.0f);

        try
        {
            networkManager.InitClient();

            networkManager.networkAddress = networkAdress;
            networkManager.networkPort = port;

            networkManager.StartClient();
        }
        catch (System.Exception)
        {
            OnFailed();
            yield break;
        }

        yield return new WaitForSeconds(1.0f);

        float time = 0.0f;
        while (networkManager.isNetworkActive)
        {
            if (networkManager.client == null)
            {
                OnFailed();
                yield break;
            }

            if (networkManager.client.isConnected)
                yield break;

            time += Time.deltaTime;
            if (time > 5.0f)
            {
                OnFailed();
                yield break;
            }

            yield return null;
        }
    }

    private void OnFailed()
    {
        NobleNetworkManager networkManager = (NobleNetworkManager)NetworkManager.singleton;
        if (networkManager.client != null)
            networkManager.StopClient();

        networkManager.StartHost();
    }
}
