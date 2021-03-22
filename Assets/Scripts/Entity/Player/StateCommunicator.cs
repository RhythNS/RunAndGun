using Mirror;

public delegate void PercentageChanged(float percentage);

public class StateCommunicator : NetworkBehaviour
{
    public bool lobbyReady = false;
    public bool levelLoaded = false;
    public bool bossAnimationFinished = false;

    public event PercentageChanged OnPercentageChanged;

    [SyncVar(hook = nameof(OnLevelLoadPercentageChanged))] private float loadPercentage;

    [Command]
    public void CmdLobbySetReady(bool ready)
    {
        if (lobbyReady == ready)
            return;

        lobbyReady = ready;
        LobbyManager.OnPlayerChangedReady();
    }

    [Command]
    public void CmdSetLevelLoadPercentage(float newPercentage)
    {
        if (levelLoaded == true)
            return;

        loadPercentage = newPercentage;
    }

    public void OnLevelLoadPercentageChanged(float oldPercentage, float newPercentage)
    {
        OnPercentageChanged.Invoke(newPercentage);
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
