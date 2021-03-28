using Mirror;
using NobleConnect.Mirror;
using TMPro;
using UnityEngine;

public class ServerInfo : PanelElement
{
    [SerializeField] private TMP_InputField ipInput;
    [SerializeField] private TMP_InputField portInput;

    public override void InnerOnShow()
    {
        NobleNetworkManager networkManager = (NobleNetworkManager)NetworkManager.singleton;

        if (networkManager.HostEndPoint != null)
        {
            ipInput.text = networkManager.HostEndPoint.Address.ToString();
            portInput.text = networkManager.HostEndPoint.Port.ToString();
        }
        else
        {
            ipInput.text = "No internet connection";
            portInput.text = "";
        }
    }
}
