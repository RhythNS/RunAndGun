using Mirror;
using NobleConnect.Mirror;
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

        networkManager.InitClient();

        networkManager.networkAddress = ipInput.text;
        networkManager.networkPort = port;

        networkManager.StartClient();

        return true;
    }
}
