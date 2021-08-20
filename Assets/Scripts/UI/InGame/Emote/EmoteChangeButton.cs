using TMPro;
using UnityEngine;

public class EmoteChangeButton : MonoBehaviour
{
    public int Id { get; private set; }

    [SerializeField] private TMP_Text text;
    
    private EmoteManager manager;

    public void Set(EmoteManager manager, string emoteLayerName, int id)
    {
        this.manager = manager;
        Id = id;
        text.text = emoteLayerName;
    }

    public void OnButtonDown()
    {
        manager.OnLayerChange(Id);
    }
}
