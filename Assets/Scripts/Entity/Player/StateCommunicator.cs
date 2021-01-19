using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateCommunicator : NetworkBehaviour
{
    public bool lobbyReady = false;
    public bool levelLoaded = false;

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
}
