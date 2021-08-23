using Mirror;
using UnityEngine;

public class WorldInDeleteOnAnimationEvent : MonoBehaviour
{
    public void AnimationFinished()
    {
        if (NetworkServer.active == true)
            NetworkServer.Destroy(gameObject);
    }
}
