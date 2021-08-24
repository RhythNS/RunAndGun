using Mirror;
using UnityEngine;

/// <summary>
/// A controller to delete the world event when the Animation event
/// "AnimationFinished" was thrown.
/// </summary>
public class WorldInDeleteOnAnimationEvent : MonoBehaviour
{
    public void AnimationFinished()
    {
        if (NetworkServer.active == true)
            NetworkServer.Destroy(gameObject);
    }
}
