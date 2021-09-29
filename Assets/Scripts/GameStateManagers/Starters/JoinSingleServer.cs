using Mirror;
using UnityEngine;

/// <summary>
/// Joins a single server if there was no current active connection.
/// </summary>
public class JoinSingleServer : MonoBehaviour
{
    private void Start()
    {
        if (NetworkClient.active == false)
            NetworkConnector.TryStartServer(false);
        else
        {
            JoinMessage joinMessage = JoinMessage.GetDefault();
            NetworkClient.Send(joinMessage);
        }

        Destroy(this);
    }
}
