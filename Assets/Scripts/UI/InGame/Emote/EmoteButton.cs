using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EmoteButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image image;

    public EmoteManager manager;
    private int emoteId;

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
