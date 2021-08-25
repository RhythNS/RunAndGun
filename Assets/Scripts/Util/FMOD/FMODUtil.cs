using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public static class FMODUtil
{
    public static void PlayOneShot(string eventName)
    {
        RuntimeManager.PlayOneShot(eventName);
    }

    public static void PlayOnTransform(string eventName, Transform transform)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventName);
        eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        eventInstance.start();
        eventInstance.release();
    }

    public static void PlayOnPosition(string eventName, Vector3 position)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventName);
        eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        eventInstance.start();
        eventInstance.release();
    }
}
