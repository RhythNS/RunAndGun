using Mirror;
using UnityEngine;

public class RAGNetworkManager : NetworkManager
{
    public override void OnStartServer()
    {
        // Register custom messages
        NetworkServer.RegisterHandler<JoinMessage>(OnJoinMessage);
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
        newPlayer.transform.position = startPositions[Random.Range(0, startPositions.Count)].position;

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
        newPlayer.name = joinMessage.name;

    }

}
