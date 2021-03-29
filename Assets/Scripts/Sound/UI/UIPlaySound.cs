using UnityEngine;

public class UIPlaySound : MonoBehaviour
{
    [SerializeField] private UISoundManager.Sound sound;

    public void Play()
    {
        UISoundManager.Instance.PlaySound(sound);
    }
}
