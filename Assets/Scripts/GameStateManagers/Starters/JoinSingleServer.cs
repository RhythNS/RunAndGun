using UnityEngine;

public class JoinSingleServer : MonoBehaviour
{
    private void Start()
    {
        NetworkConnector.TryStartServer(true);
        Destroy(this);
    }
}
