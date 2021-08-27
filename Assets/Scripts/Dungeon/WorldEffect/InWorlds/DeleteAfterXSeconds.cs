using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

/// <summary>
/// Deletes the gameobject after x seconds.
/// </summary>
public class DeleteAfterXSeconds : MonoBehaviour
{
    [SerializeField]
    private float seconds = 5.0f;

    void Start()
    {
        if (!NetworkServer.active)
            return;

        StartCoroutine(DeleteSelf());
    }

    private IEnumerator DeleteSelf()
    {
        yield return new WaitForSeconds(seconds);

        NetworkServer.Destroy(gameObject);
    }
}
