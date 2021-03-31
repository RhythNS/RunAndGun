using FMODUnity;
using UnityEngine;

public class OneShotPlayer : MonoBehaviour
{
    [SerializeField] [EventRef] private string eventName;

    public void Play()
    {
        RuntimeManager.PlayOneShot(eventName);
    }
}
