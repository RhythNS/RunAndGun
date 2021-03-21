using Mirror;
using NobleConnect.Mirror;
using UnityEngine;

public class RAGNetworkManager : NobleNetworkManager
{
    // TODO: Check all messages. If something is wrong, return to main menu

    public override void OnStartServer()
    {
        base.OnStartServer();

        // Register custom messages
        NetworkServer.RegisterHandler<JoinMessage>(OnJoinMessage);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        NetworkClient.RegisterHandler<StartGameMessage>(OnStartGameMessage);
        NetworkClient.RegisterHandler<ReturnToLobbyMessage>(OnReturnToLobbyMessage);
        NetworkClient.RegisterHandler<DoorMessage>(OnDoorsMessage);
        NetworkClient.RegisterHandler<GenerateLevelMessage>(OnGenerateLevelMessage);
        NetworkClient.RegisterHandler<BossSpawnMessage>(OnBossSpawnMessage);
        NetworkClient.RegisterHandler<EveryoneLoadedMessage>(OnEveryoneLoadedMessage);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        JoinMessage message = new JoinMessage
        {
            name = Config.Instance.playerName,
            characterType = Config.Instance.selectedPlayerType
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

            NetworkServer.ReplacePlayerForConnection(connection, newPlayer.gameObject);

            Destroy(oldPlayer);
        }
        else // They joined for the first time
        {
            NetworkServer.AddPlayerForConnection(connection, newPlayer.gameObject);
        }
        newPlayer.userName = joinMessage.name;

    }

    private void OnStartGameMessage(StartGameMessage startGameMessage)
    {
        LobbyLevel.Instance.Hide();

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
        LobbyLevel.Instance.Show();
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
    }

    private void OnEveryoneLoadedMessage(EveryoneLoadedMessage everyoneLoadedMessage)
    {
        UIManager.Instance.HideLevelLoadScreen();
    }
}
