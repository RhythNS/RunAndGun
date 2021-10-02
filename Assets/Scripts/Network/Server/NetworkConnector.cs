using Mirror;
using NobleConnect.Mirror;
using System;
using System.Collections;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// Helper class for connecting to or hosting a server.
/// </summary>
public class NetworkConnector : MonoBehaviour
{
    #region ConnectToServer
    public static IEnumerator TryConnectToServer(string networkAdress, ushort port, Action onSuccess = null, Action onFailure = null)
    {
        NobleNetworkManager networkManager = (NobleNetworkManager)NetworkManager.singleton;
        networkManager.StopHost();

        yield return new WaitForSeconds(1.0f);

        try
        {
            networkManager.InitClient();

            networkManager.networkAddress = networkAdress;
            networkManager.networkPort = port;

            networkManager.StartClient();
        }
        catch (Exception)
        {
            TryConnectToServerFailed(onFailure);
            yield break;
        }

        yield return new WaitForSeconds(1.0f);

        float time = 0.0f;
        while (networkManager.isNetworkActive)
        {
            if (networkManager.client == null)
            {
                TryConnectToServerFailed(onFailure);
                yield break;
            }

            if (networkManager.client.isConnected)
            {
                onSuccess?.Invoke();
                yield break;
            }

            time += Time.deltaTime;
            if (time > 5.0f)
            {
                TryConnectToServerFailed(onFailure);
                yield break;
            }

            yield return null;
        }
    }

    public static void DisconnectClient()
    {
        (NetworkManager.singleton as RAGNetworkManager).ExpectingDisconnect = true;
        NetworkClient.Disconnect();
        NetworkManager.singleton.StopServer();
    }

    public static void TryConnectToServerFailed(Action onFailure)
    {
        Debug.LogWarning("Could not connect to server!");

        TryStartServer(true);

        onFailure?.Invoke();
    }
    #endregion

    #region StartServer
    public static IEnumerator RestartServerWithInternetConnection(Action onSuccess)
    {
        NobleNetworkManager networkManager = (NobleNetworkManager)NetworkManager.singleton;
        networkManager.StopHost();
        while (NetworkServer.active)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        if (TryStartServer(false) == false)
        {
            OnRestartFailed();
            yield break;
        }
        Debug.Log("Waiting for endpoint");
        while (((NobleNetworkManager)NetworkManager.singleton).HostEndPoint == null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log("Noble network established!");
        onSuccess?.Invoke();
    }

    private static void OnRestartFailed()
    {
        UIManager.Instance.ShowNotification("Could not connect to the internet!");
        TryStartServer(true);
    }

    /// <summary>
    /// Tries to start a server.
    /// </summary>
    /// <param name="lanOnly">Wheter the server should be lan only.</param>
    public static bool TryStartServer(bool lanOnly)
    {
        NobleNetworkManager networkManager = (NobleNetworkManager)NetworkManager.singleton;

        // Try to start server. If the port is already in use, catch the error and try a different one.
        int startPort = networkManager.networkPort;
        for (int i = 0; i < 5; i++)
        {
            try
            {
                networkManager.networkPort = startPort + i;

                if (lanOnly)
                    networkManager.StartHostLANOnly();
                else
                    networkManager.StartHost();

                return true;
            }
            catch (SocketException se)
            {
                if (se.SocketErrorCode == SocketError.AddressAlreadyInUse)
                    continue;

                // unknown error
                Debug.LogException(se);
            }
        }

        return false;
    }
    #endregion
}
