using Mirror;

public class StateCommunicator : NetworkBehaviour
{
    public bool lobbyReady = false;
    public bool levelLoaded = false;
    public bool bossAnimationFinished = false;

    [Command]
    public void CmdLobbySetReady(bool ready)
    {
        if (lobbyReady == ready)
            return;

        lobbyReady = ready;
        LobbyManager.OnPlayerChangedReady();
    }

    [Command]
    public void CmdLevelSetLoaded(bool loaded)
    {
        if (levelLoaded == loaded)
            return;

        levelLoaded = loaded;
        GameManager.OnPlayerLoadedLevelChanged();
    }

    [Command]
    public void CmdBossAnimationFinished()
    {
        if (bossAnimationFinished)
            return;

        bossAnimationFinished = true;
    }
}
