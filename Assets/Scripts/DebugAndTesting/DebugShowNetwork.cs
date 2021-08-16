using Mirror;
using NobleConnect.Mirror;
using UnityEngine;

/// <summary>
/// Shows the ip and host on canvas.
/// </summary>
public class DebugShowNetwork : MonoBehaviour
{
    private void OnGUI()
    {
        NobleNetworkManager networkManager = (NobleNetworkManager)NetworkManager.singleton;
        if (networkManager.HostEndPoint != null)
        {
            GUI.Label(new Rect(10, 10, 150, 22), "Host IP:");
            GUI.TextField(new Rect(170, 10, 420, 22), networkManager.HostEndPoint.Address.ToString(), "Label");
            GUI.Label(new Rect(10, 37, 150, 22), "Host Port:");
            GUI.TextField(new Rect(170, 37, 160, 22), networkManager.HostEndPoint.Port.ToString(), "Label");
        }
    }
}
