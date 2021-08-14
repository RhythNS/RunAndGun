using Mirror;
using NobleConnect.Mirror;
using System.Collections.Generic;
using UnityEngine;

public class RAGNetworkManager : NobleNetworkManager
{
    // TODO: Check all messages. If something is wrong, return to main menu

    public bool IsLanOnly => isLANOnly;

    public override void OnStartServer()
    {
        base.OnStartServer();
        
        // Register custom messages
        NetworkServer.RegisterHandler<JoinMessage>(OnJoinMessage);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        NetworkPool[] networkPools = GlobalsDict.Instance.PoolObject.GetComponents<NetworkPool>();
        for (int i = 0; i < networkPools.Length; i++)
            networkPools[i].Setup();

        NetworkClient.RegisterHandler<StartGameMessage>(OnStartGameMessage);
        NetworkClient.RegisterHandler<ReturnToLobbyMessage>(OnReturnToLobbyMessage);
        NetworkClient.RegisterHandler<DoorMessage>(OnDoorsMessage);
        NetworkClient.RegisterHandler<GenerateLevelMessage>(OnGenerateLevelMessage);
        NetworkClient.RegisterHandler<BossSpawnMessage>(OnBossSpawnMessage);
        NetworkClient.RegisterHandler<EveryoneLoadedMessage>(OnEveryoneLoadedMessage);
        NetworkClient.RegisterHandler<MiniMapNewRoomMessage>(OnMiniMapNewRoomMessage);
        NetworkClient.RegisterHandler<EmoteMessage>(OnEmoteMessage);
        NetworkClient.RegisterHandler<GameOverMessage>(OnGameOverMessage);
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

        Debug.Log("Someone connected to the server!");
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        
        JoinMessage message = new JoinMessage
        {
            name = Config.Instance.PlayerName,
            characterType = Config.Instance.SelectedPlayerType,
            password = Config.Instance.password
        };
        conn.Send(message);
    }

    /// <summary>
    /// JoinMessage is sent by clients when a player joins for the first time or
    /// when a player wants to change their roll.
    /// </summary>
    /// <param name="connection">The client who sent the message.</param>
    /// <param name="joinMessage">The message.</param>
    private void OnJoinMessage(NetworkConnection connection, JoinMessage joinMessage)
    {
        Player newPlayer = Instantiate(CharacterDict.Instance.GetPlayerForType(joinMessage.characterType));
        newPlayer.SmoothSync.setPosition(
            RandomUtil.Element(startPositions).position,
            true
            );
        newPlayer.playerId = connection.connectionId;

        // Do they want to replace their character?
        if (connection.identity?.gameObject)
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
                    Debug.LogError("Old player did not have a connection and there are no places remaining!");
                    connection.Disconnect();
                    Destroy(oldPlayer);
                    Destroy(newPlayer);
                    return;
                }
                newPlayer.playerIndex = index.Value;
            }

            NetworkServer.ReplacePlayerForConnection(connection, newPlayer.gameObject);

            Destroy(oldPlayer);
        }
        else // They joined for the first time
        {
            string reqPassword = Config.Instance.password;
            if (reqPassword.Length != 0 && joinMessage.password.Equals(reqPassword) == false)
            {
                Debug.LogError("New player joined with wrong password!");
                connection.Disconnect();
                Destroy(newPlayer);
                return;
            }

            int? index = GetPlayerIndex(connection.connectionId);
            if (index == null)
            {
                Debug.LogError("New player joined a full game!");
                connection.Disconnect();
                Destroy(newPlayer);
                return;
            }
            newPlayer.playerIndex = index.Value;

            NetworkServer.AddPlayerForConnection(connection, newPlayer.gameObject);
        }
        newPlayer.entityName = joinMessage.name;
    }

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
        UIManager.Instance.ShowLevelLoadScreen();
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
}
