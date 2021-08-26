using FMODUnity;
using UnityEngine;

/// <summary>
/// Plays a fmod one shot.
/// </summary>
public class OneShotPlayer : MonoBehaviour
{
    [SerializeField] [EventRef] private string eventName;

    public void Play()
    {
        RuntimeManager.PlayOneShot(eventName);
    }
}
