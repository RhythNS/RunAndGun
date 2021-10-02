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
#if UNITY_EDITOR
            NetworkConnector.TryStartServer(false);
#else
            NetworkConnector.TryStartServer(true);
#endif
        else
        {
            JoinMessage joinMessage = JoinMessage.GetDefault();
            NetworkClient.Send(joinMessage);
        }

        Destroy(this);
    }
}
