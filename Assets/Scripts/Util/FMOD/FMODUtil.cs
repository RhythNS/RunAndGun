using FMOD.Studio;
using FMODUnity;
using UnityEngine;

/// <summary>
/// Helper class for methods related to FMOD.
/// </summary>
public static class FMODUtil
{
    /// <summary>
    /// Plays a one shot. Should probably be a 2D sound.
    /// </summary>
    /// <param name="eventName">The event to be played.</param>
    public static void PlayOneShot(string eventName)
    {
        RuntimeManager.PlayOneShot(eventName);
    }

    /// <summary>
    /// Plays a one shot attached to a transform.
    /// </summary>
    /// <param name="eventName">The event to be played.</param>
    /// <param name="transform">The transform to where the sound should be attached to.</param>
    public static void PlayOnTransform(string eventName, Transform transform)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventName);
        eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        eventInstance.start();
        eventInstance.release();
    }

    /// <summary>
    /// Plays a one shot on a given position.
    /// </summary>
    /// <param name="eventName">The event to be played.</param>
    /// <param name="position">The position of where the sound should be played from.</param>
    public static void PlayOnPosition(string eventName, Vector3 position)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventName);
        eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        eventInstance.start();
        eventInstance.release();
    }
}
