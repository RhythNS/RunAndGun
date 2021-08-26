using UnityEngine;

/// <summary>
/// UI Element for showing the emote panel.
/// </summary>
public class EmoteShowButton : MonoBehaviour
{
    public void Toggle()
    {
        UIManager.Instance.ToggleEmotePanel();
    }
}
