using UnityEngine;

public class SendEmoteTester : MonoBehaviour
{
    [SerializeField] private int emoteID;

    private void Start()
    {
        Debug.Log("Send test message with F9");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            Player.LocalPlayer.EmoteCommunicator.CmdSend(emoteID);
        }
    }
}
