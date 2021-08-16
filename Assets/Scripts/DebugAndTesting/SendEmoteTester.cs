using UnityEngine;

/// <summary>
/// Sends a specified emote as the local player to all other players.
/// </summary>
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
