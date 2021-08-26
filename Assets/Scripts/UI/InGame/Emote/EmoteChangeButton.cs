using TMPro;
using UnityEngine;

/// <summary>
/// UI Button for changing the emote button layer.
/// </summary>
public class EmoteChangeButton : MonoBehaviour
{
    public int Id { get; private set; }

    [SerializeField] private TMP_Text text;
    
    private EmoteManager manager;

    /// <summary>
    /// Sets all values.
    /// </summary>
    /// <param name="manager">A reference to the emote manager.</param>
    /// <param name="emoteLayerName">The name of the layer.</param>
    /// <param name="id">The id of the layer.</param>
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
