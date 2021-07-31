using UnityEngine;

public class JoinSingleServer : MonoBehaviour
{
    private void Start()
    {
        NetworkConnector.TryStartServer(false);
        Destroy(this);
    }
}
