using UnityEngine;

public class EmoteShowButton : MonoBehaviour
{
    public void Toggle()
    {
        UIManager.Instance.ToggleEmotePanel();
    }
}
