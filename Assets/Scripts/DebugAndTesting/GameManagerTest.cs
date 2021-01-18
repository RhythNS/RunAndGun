using Mirror;
using System.Collections;
using UnityEngine;

public class GameManagerTest : NetworkBehaviour
{
    public override void OnStartServer()
    {
        StartCoroutine(StartLevel());
    }

    private IEnumerator StartLevel()
    {
        yield return new WaitForSeconds(1.0f);
        GameManager.OnLevelLoaded();
    }
}
