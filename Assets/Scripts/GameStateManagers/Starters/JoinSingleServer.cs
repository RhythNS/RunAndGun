using Mirror;
using UnityEngine;

public class JoinSingleServer : MonoBehaviour
{
    private void Start()
    {
        if (NetworkClient.active == false)
            NetworkConnector.TryStartServer(false);
        else
        {
            JoinMessage joinMessage = new JoinMessage()
            {
                characterType = Config.Instance.SelectedPlayerType,
                name = Config.Instance.PlayerName
            };
            NetworkClient.Send(joinMessage);
        }

        Destroy(this);
    }
}
