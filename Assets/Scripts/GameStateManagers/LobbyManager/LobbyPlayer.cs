using Mirror;

public class LobbyPlayer : NetworkBehaviour
{
    public bool isReady = false;

    public override void OnStartServer()
    {
        LobbyManager.Register(this);
    }

    private void OnDestroy()
    {
        LobbyManager.DeRegister(this);
    }

    [Command]
    public void CmdSetReady(bool ready)
    {
        if (isReady == ready)
            return;

        isReady = ready;
        LobbyManager.OnPlayerChangedReady();
    }
}
