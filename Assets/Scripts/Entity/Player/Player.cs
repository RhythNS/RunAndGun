using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public CharacterType CharacterType => characterType;
    [SerializeField] private CharacterType characterType;

    /// <summary>
    /// Called when player is created and assigend to a connection.
    /// </summary>
    /// <param name="name">The username of the connection.</param>
    public void Init(string name)
    {
        gameObject.name = name;

        // Add input method if I am local player.
        if (isLocalPlayer)
            Input.AttachInput(gameObject);
    }

    [Command]
    public void CmdValidate()
    {

    }
}
