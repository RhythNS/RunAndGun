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

        gameObject.AddComponent<LoadingPlayer>();
    }
}
