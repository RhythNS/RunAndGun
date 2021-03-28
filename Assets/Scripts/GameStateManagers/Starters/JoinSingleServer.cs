using Mirror;
using NobleConnect.Mirror;
using UnityEngine;

public class JoinSingleServer : MonoBehaviour
{
    private void Start()
    {
        NobleNetworkManager networkManager = (NobleNetworkManager)NetworkManager.singleton;

        if (networkManager.isNetworkActive)
            return;

        networkManager.StartHost();

        Destroy(this);
    }
}
