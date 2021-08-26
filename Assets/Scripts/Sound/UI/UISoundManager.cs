using FMODUnity;
using UnityEngine;

/// <summary>
/// Sound manager for playing ui sounds through fmod.
/// </summary>
public class UISoundManager : MonoBehaviour
{
    public enum Sound
    {
        TextboxTyped, TextboxBackspace, Confirm, Cancel
    }

    [System.Serializable]
    public struct NameForSound
    {
        public Sound sound;
        [EventRef] public string eventName;
    }

    public static UISoundManager Instance { get; private set; }

    [SerializeField] private NameForSound[] sounds;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("UISoundManager already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Plays a sound.
    /// </summary>
    /// <param name="sound">The sound to be played.</param>
    public void PlaySound(Sound sound)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].sound == sound)
            {
                FMODUtil.PlayOneShot(sounds[i].eventName);
                return;
            }
        }
        throw new System.Exception("Sound " + sound + " not found!");
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

}
