using UnityEngine;

/// <summary>
/// Helper class for playing ui sounds through fmod.
/// </summary>
public class UIPlaySound : MonoBehaviour
{
    [SerializeField] private UISoundManager.Sound sound;

    public void Play()
    {
        UISoundManager.Instance.PlaySound(sound);
    }
}
