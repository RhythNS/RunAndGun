using Mirror;
using System.Collections;
using UnityEngine;

public class AddComponentTest : NetworkBehaviour
{
    public override void OnStartServer()
    {
        StartCoroutine(WaitForPress());
    }

    private IEnumerator WaitForPress()
    {
        while (true)
        {
            if (Input.GetKey(KeyCode.X))
                break;
            yield return null;
        }
        Player[] players = FindObjectsOfType<Player>();
        for (int i = 0; i < players.Length; i++)
        {
            players[i].gameObject.AddComponent<LoadingPlayer>();
        }
    }
}
