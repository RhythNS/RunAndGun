using Mirror;
using NobleConnect.Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages incoming messages and the connection processes.
/// </summary>
public class RAGNetworkManager : NobleNetworkManager
{
    // TODO: Check all messages. If something is wrong, return to main menu

    public bool IsLanOnly => isLANOnly;
    public bool ExpectingDisconnect { get; set; } = false;
    private DisconnectMessage.Type expectingDisconnectType = DisconnectMessage.Type.Unknown;

    #region Client
    public override void OnStartClient()
    {
        base.OnStartClient();

        NetworkPool[] networkPools = GlobalsDict.Instance.PoolObject.GetComponents<NetworkPool>();
        for (int i = 0; i < networkPools.Length; i++)
            networkPools[i].Setup();

        // Register all message handlers.
        NetworkClient.RegisterHandler<StartGameMessage>(OnStartGameMessage);
        NetworkClient.RegisterHandler<ReturnToLobbyMessage>(OnReturnToLobbyMessage);
        NetworkClient.RegisterHandler<DoorMessage>(OnDoorsMessage);
        NetworkClient.RegisterHandler<GenerateLevelMessage>(OnGenerateLevelMessage);
        NetworkClient.RegisterHandler<BossSpawnMessage>(OnBossSpawnMessage);
        NetworkClient.RegisterHandler<BossDefeatedMessage>(OnBossDefeatedMessage);
        NetworkClient.RegisterHandler<EveryoneLoadedMessage>(OnEveryoneLoadedMessage);
        NetworkClient.RegisterHandler<MiniMapNewRoomMessage>(OnMiniMapNewRoomMessage);
        NetworkClient.RegisterHandler<EmoteMessage>(OnEmoteMessage);
        NetworkClient.RegisterHandler<GameOverMessage>(OnGameOverMessage);
        NetworkClient.RegisterHandler<DisconnectMessage>(OnDisconnectMessage);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        JoinMessage joinMessage = JoinMessage.GetDefault();
        conn.Send(joinMessage);

        ExpectingDisconnect = false;
        expectingDisconnectType = DisconnectMessage.Type.Unknown;
    }

    private void OnDisconnectMessage(NetworkConnection connection, DisconnectMessage disconnectMessage)
    {
        expectingDisconnectType = disconnectMessage.type;
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Debug.Log("Client disconnected! Waiting for shutdown!");

        base.OnClientDisconnect(conn);

        if (NetworkServer.active == false)
            RAGMatchmaker.Instance.Disconnect();

        StartCoroutine(WaitForDisconnect());

        if (ExpectingDisconnect == true)
            return;

        NetworkIdentity[] identities = FindObjectsOfType<NetworkIdentity>();
        for (int i = 0; i < identities.Length; i++)
        {
            Destroy(identities[i].gameObject);
        }

        if (UIManager.Instance.IsLoadingScreenActive() == true)
        {
            DungeonCreator.Instance.StopAllCoroutines();
            UIManager.Instance.HideLevelLoadScreen();
        }
    }

    private IEnumerator WaitForDisconnect()
    {
        while (client != null)
        {
            yield return null;
        }

        if (ExpectingDisconnect)
            OnPlannedDisconnect();
        else
            OnSuddenDisconnect();

        ExpectingDisconnect = false;
    }

    private void OnSuddenDisconnect()
    {
        string reason;
        switch (expectingDisconnectType)
        {
            case DisconnectMessage.Type.PasswordWrong:
                reason = "Could not join server! Entered password is wrong!";
                break;
            case DisconnectMessage.Type.ServerFull:
                reason = "Could not join server! The server is already full!";
                break;
            case DisconnectMessage.Type.Kicked:
                reason = "You were kicked from the server!";
                break;
            case DisconnectMessage.Type.JoinedGameInProgress:
                reason = "Could not join server! Game already in progress!";
                break;
            default:
                reason = "Lost connection to the server!";
                break;
        }

        if (RegionDict.Instance.Region == Region.Lobby)
        {
            NetworkConnector.TryStartServer(false);
            UIManager.Instance.ShowNotification(reason);
            return;
        }
        UIManager.Instance.OptionsManager.ShowLostConnectionScreen(expectingDisconnectType == DisconnectMessage.Type.Kicked);
    }

    private void OnPlannedDisconnect()
    {
        if (RegionDict.Instance.Region == Region.Lobby)
        {
            NetworkConnector.TryStartServer(false);
            return;
        }
        GlobalsDict.Instance.StartCoroutine(RegionSceneLoader.Instance.LoadScene(Region.Lobby));
    }
    #endregion

    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();

        // Register custom messages
        NetworkServer.RegisterHandler<JoinMessage>(OnJoinMessage);
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

        Debug.Log("Someone connected to the server!");
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Player player = PlayersDict.Instance.GetPlayerWithID(conn.connectionId);

        if (player != null && RegionDict.Instance.Region != Region.Lobby)
        {
            NetworkServer.RemovePlayerForConnection(conn, false);
        }

        base.OnServerDisconnect(conn);
    }

    /// <summary>
    /// JoinMessage is sent by clients when a player joins for the first time or
    /// when a player wants to change their roll.
    /// </summary>
    /// <param name="connection">The client who sent the message.</param>
    /// <param name="joinMessage">The message.</param>
    private void OnJoinMessage(NetworkConnection connection, JoinMessage joinMessage)
    {
        Player oldPlayer = PlayersDict.Instance.GetPlayerWithUniqueIdentifier(joinMessage.uniqueIdentifier);
        // Was the player connected to the server before?
        if (oldPlayer != null)
        {
            // Is the player still connected?
            if (connection.identity?.gameObject == oldPlayer.gameObject)
            {
                JoinReplaceCharacter(connection, JoinCreateNewPlayer(connection, joinMessage), joinMessage);
                return;
            }

            // Player is reconnecting
            JoinReconnect(connection, oldPlayer);
            return;
        }

        // Is the game already in progress?
        if (RegionDict.Instance.Region != Region.Lobby)
        {
            DelayedDisconnect(connection, DisconnectMessage.Type.JoinedGameInProgress);
            return;
        }

        // Player joined for the first time and we are in the lobby scene.
        Player newPlayer = JoinCreateNewPlayer(connection, joinMessage);
        JoinFirstTime(connection, newPlayer, joinMessage);
        RAGMatchmaker.Instance.UpdatePlayerCount(PlayersDict.Instance.Players.Count);
    }

    private Player JoinCreateNewPlayer(NetworkConnection connection, JoinMessage joinMessage)
    {
        Player newPlayer = Instantiate(CharacterDict.Instance.GetPlayerForType(joinMessage.characterType));
        newPlayer.SmoothSync.setPosition(
            RandomUtil.Element(startPositions).position,
            true
            );
        newPlayer.playerId = connection.connectionId;
        newPlayer.uniqueIdentifier = joinMessage.uniqueIdentifier;
        return newPlayer;
    }

    private void JoinReplaceCharacter(NetworkConnection connection, Player newPlayer, JoinMessage joinMessage)
    {
        GameObject oldPlayer = connection.identity.gameObject;
        Player castPlayer = oldPlayer.GetComponent<Player>();

        if (castPlayer != null)
            newPlayer.playerIndex = castPlayer.playerIndex;
        else
        {
            int? index = GetPlayerIndex(connection.connectionId);
            if (index == null)
            {
                DelayedDisconnect(connection, DisconnectMessage.Type.ServerFull);

                Destroy(oldPlayer);
                Destroy(newPlayer);
                return;
            }
            newPlayer.playerIndex = index.Value;
        }

        NetworkServer.ReplacePlayerForConnection(connection, newPlayer.gameObject);

        Destroy(oldPlayer);
        newPlayer.entityName = joinMessage.name;
    }

    private void JoinFirstTime(NetworkConnection connection, Player newPlayer, JoinMessage joinMessage)
    {
        string reqPassword = Config.Instance.password;
        if (reqPassword.Length != 0 && joinMessage.password.Equals(reqPassword) == false)
        {
            DelayedDisconnect(connection, DisconnectMessage.Type.PasswordWrong);

            Destroy(newPlayer);
            return;
        }

        int? index = GetPlayerIndex(connection.connectionId);
        if (index == null)
        {
            DelayedDisconnect(connection, DisconnectMessage.Type.ServerFull);

            Destroy(newPlayer);
            return;
        }
        newPlayer.playerIndex = index.Value;

        NetworkServer.AddPlayerForConnection(connection, newPlayer.gameObject);
        newPlayer.entityName = joinMessage.name;
    }

    private void JoinReconnect(NetworkConnection connection, Player oldPlayer)
    {
        NetworkServer.AddPlayerForConnection(connection, oldPlayer.gameObject);
        oldPlayer.playerId = connection.connectionId;
        
        StartGameMessage sgm = new StartGameMessage()
        {
            gameMode = GameManager.gameMode,
            levelSeed = GameManager.gameMode.seed
        };
        connection.Send(sgm);

        Debug.Log("Current level " + GameManager.currentLevel);
        GenerateLevelMessage glm = new GenerateLevelMessage()
        {
            levelNumber = GameManager.currentLevel,
            reconnecting = true,
            region = RegionDict.Instance.Region
        };
        connection.Send(glm);
    }

    /// <summary>
    /// Gets a player index for a new player or a player that reconnected.
    /// </summary>
    /// <param name="ignoreNetID">Playerid that can be ignored. Can be null.</param>
    /// <returns>The new player index. Can be null.</returns>
    private int? GetPlayerIndex(int? ignoreNetID)
    {
        List<Player> players = PlayersDict.Instance.Players;
        int? indexToReturn = null;
        for (int i = 0; i < 4; i++)
        {
            indexToReturn = i;
            for (int j = 0; j < players.Count; j++)
            {
                if (ignoreNetID != null && players[j].playerId == ignoreNetID.Value)
                    continue;
                if (players[j].playerIndex == i)
                {
                    indexToReturn = null;
                    break;
                }
            }

            if (indexToReturn != null)
                return indexToReturn;
        }
        return indexToReturn;
    }

    private void DelayedDisconnect(NetworkConnection connection, DisconnectMessage.Type? type)
    {
        if (type != null)
        {
            DisconnectMessage dm = new DisconnectMessage() { type = type.Value };
            connection.Send(dm);
        }

        StartCoroutine(InnerDelayedDisconnect(connection));
    }

    private IEnumerator InnerDelayedDisconnect(NetworkConnection connection)
    {
        yield return new WaitForSeconds(0.1f);
        connection.Disconnect();
        RAGMatchmaker.Instance.UpdatePlayerCount(PlayersDict.Instance.Players.Count);
    }
    #endregion

    #region NetworkMessages
    private void OnStartGameMessage(StartGameMessage startGameMessage)
    {
        LobbyLevel.Instance.Hide();
        MusicManager.Instance.ChangeState(MusicManager.State.None);

        if (!Player.LocalPlayer || Player.LocalPlayer.isServer)
        {
            GameManager.gameMode.Init(startGameMessage.levelSeed);
        }
        else
        {
            startGameMessage.gameMode.Init(startGameMessage.levelSeed);
            GameManager.gameMode = startGameMessage.gameMode;
        }
    }

    private void OnGenerateLevelMessage(GenerateLevelMessage generateLevelMessage)
    {
        UIManager.Instance.ShowLevelLoadScreen(generateLevelMessage.reconnecting);
        DungeonDict.Instance.ClearRooms();
        RegionSceneLoader loader = RegionSceneLoader.Instance;
        new ExtendedCoroutine(this, loader.LoadScene(generateLevelMessage), loader.LoadLevel, true);
    }

    private void OnReturnToLobbyMessage(ReturnToLobbyMessage returnToLobbyMessage)
    {
        StartCoroutine(RegionSceneLoader.Instance.LoadScene(Region.Lobby));
        StartCoroutine(DungeonCreator.Instance.ClearPreviousDungeon());
    }

    private void OnDoorsMessage(DoorMessage doorMessage)
    {
        if (!DungeonDict.Instance || !DungeonDict.Instance.IsIdValid(doorMessage.roomId))
        {
            Debug.LogWarning("Recieved door message with invalid id! (" + doorMessage.roomId + ")");
            return;
        }

        if (doorMessage.open)
            DungeonDict.Instance.Get(doorMessage.roomId).OnOpenDoors();
        else
            DungeonDict.Instance.Get(doorMessage.roomId).OnCloseDoors();
    }

    private void OnBossSpawnMessage(BossSpawnMessage bossSpawnMessage)
    {
        if (!DungeonDict.Instance || !DungeonDict.Instance.IsIdValid(bossSpawnMessage.id))
        {
            Debug.LogWarning("Recieved door message with invalid id! (" + bossSpawnMessage.id + ")");
            return;
        }

        (DungeonDict.Instance.Get(bossSpawnMessage.id) as BossRoom).StartBossAnimation(bossSpawnMessage);
        MusicManager.Instance.ChangeState(MusicManager.State.Boss);
    }

    private void OnBossDefeatedMessage(BossDefeatedMessage bossDefeatedMessage)
    {
        MusicManager.Instance.ChangeState(MusicManager.State.Dungeon);
    }

    private void OnEveryoneLoadedMessage(EveryoneLoadedMessage everyoneLoadedMessage)
    {
        UIManager.Instance.HideLevelLoadScreen();
        MusicManager.Instance.ChangeState(MusicManager.State.Dungeon);
    }

    private void OnMiniMapNewRoomMessage(MiniMapNewRoomMessage miniMapNewRoomMessage)
    {
        if (DungeonDict.Instance.IsIdValid(miniMapNewRoomMessage.roomId) == false)
            return;
        MiniMapManager.Instance.OnNewRoomEntered(DungeonDict.Instance.Get(miniMapNewRoomMessage.roomId));
    }

    private void OnEmoteMessage(EmoteMessage emoteMessage)
    {
        if (UIManager.Instance)
            UIManager.Instance.OnPlayerEmoted(emoteMessage);
    }

    private void OnGameOverMessage(GameOverMessage gameOverMessage)
    {
        GameOverScreen.Instance.Set(gameOverMessage.statsTransmission);
    }
    #endregion
}
