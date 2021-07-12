using Mirror;
using NobleConnect.Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public static void TryConnectToServerFailed(Action onFailure)
    {
        Debug.LogWarning("Could not connect to server!");

        TryStartServer(true);

        onFailure?.Invoke();
    }
    #endregion

    #region StartServer
    public static void TryStartServer(bool lanOnly, Action onSuccess = null, Action onFailure = null)
    {
        NobleNetworkManager networkManager = (NobleNetworkManager)NetworkManager.singleton;

        if (networkManager.isNetworkActive == true)
        {
            networkManager.StopClient();
            networkManager.StopServer();
        }

        if (lanOnly == true)
            networkManager.StartHostLANOnly();
        else
            networkManager.StartHost();
    }
    #endregion
}
