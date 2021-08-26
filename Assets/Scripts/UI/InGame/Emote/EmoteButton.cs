using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI element for sending an emote.
/// </summary>
public class EmoteButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image image;

    public EmoteManager manager;
    private int emoteId;

    /// <summary>
    /// Inits all values.
    /// </summary>
    /// <param name="emote">The emote of this button.</param>
    public void Set(EmoteDict.Emote emote)
    {
        emoteId = emote.id;
        image.sprite = emote.sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        manager.OnEmote(emoteId);
    }
}
