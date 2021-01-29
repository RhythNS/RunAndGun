using Mirror;
using NobleConnect.Mirror;
using UnityEngine;

public class RAGNetworkManager : NobleNetworkManager
{
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

    private void OnStartGameMessage(NetworkConnection connection, StartGameMessage startGameMessage)
    {
        LobbyLevel.Instance.Hide();
        DungeonCreator.Instance.CreateDungeon(startGameMessage.levelSeed);
    }

    private void OnReturnToLobbyMessage(NetworkConnection connection, ReturnToLobbyMessage returnToLobbyMessage)
    {
        LobbyLevel.Instance.Show();
    }

    private void OnDoorsMessage(NetworkConnection connection, DoorMessage doorMessage)
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

}
