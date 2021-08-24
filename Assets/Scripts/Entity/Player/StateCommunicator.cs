using Mirror;

/// <summary>
/// Helper class for issuing commands or updating on local progress to the server.
/// </summary>
public class StateCommunicator : NetworkBehaviour
{
    public bool lobbyReady = false;
    public bool levelLoaded = false;
    public bool bossAnimationFinished = false;

    public event PercentageChanged OnPercentageChanged;

    [SyncVar(hook = nameof(OnLevelLoadPercentageChanged))] private float loadPercentage;

    /// <summary>
    /// Called to update wheter the player is or is not ready to transition from the lobby into the acctual game.
    /// </summary>
    [Command]
    public void CmdLobbySetReady(bool ready)
    {
        if (lobbyReady == ready)
            return;

        lobbyReady = ready;
        LobbyManager.OnPlayerChangedReady();
    }

    /// <summary>
    /// Called to update the players loading progress.
    /// </summary>
    [Command]
    public void CmdSetLevelLoadPercentage(float newPercentage)
    {
        if (levelLoaded == true)
            return;

        loadPercentage = newPercentage;
    }

    public void OnLevelLoadPercentageChanged(float oldPercentage, float newPercentage)
    {
        OnPercentageChanged?.Invoke(newPercentage);
    }

    /// <summary>
    /// Called to update wheter the player has finished loading the current level.
    /// </summary>
    [Command]
    public void CmdLevelSetLoaded(bool loaded)
    {
        if (levelLoaded == loaded)
            return;

        levelLoaded = loaded;
        GameManager.OnPlayerLoadedLevelChanged();
    }

    /// <summary>
    /// Called when the boss animation has finished playing.
    /// </summary>
    [Command]
    public void CmdBossAnimationFinished()
    {
        if (bossAnimationFinished)
            return;

        bossAnimationFinished = true;
    }

    /// <summary>
    /// Call to change the name of the player.
    /// </summary>
    [Command]
    public void CmdChangeName(string newName)
    {
        GetComponent<Player>().entityName = newName;
    }
}
