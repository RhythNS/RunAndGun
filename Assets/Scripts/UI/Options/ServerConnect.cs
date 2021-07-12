using TMPro;
using UnityEngine;

public class ServerConnect : PanelElement
{
    [SerializeField] private TMP_InputField ipInput;
    [SerializeField] private TMP_InputField portInput;

    public override bool InnerOnConfirm()
    {
        if (ushort.TryParse(portInput.text, out ushort port) == false)
            return false;

        Config.Instance.StartCoroutine(NetworkConnector.TryConnectToServer(ipInput.text, port));

        return true;
    }
}
