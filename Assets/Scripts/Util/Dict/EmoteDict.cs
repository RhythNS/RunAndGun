using UnityEngine;

public class EmoteDict : MonoBehaviour
{
    [System.Serializable]
    public struct EmoteLayer
    {
        public string name;
        public Emote[] emotes;
    }

    [System.Serializable]
    public struct Emote
    {
        public string identifier;
        public Sprite sprite;
        public int id;
    }

    public static EmoteDict Instance { get; private set; }

    public EmoteLayer[] EmoteLayers => emoteLayers;
    [SerializeField] private EmoteLayer[] emoteLayers;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("EmoteDict already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;

        if (emoteLayers.Length > 9)
            Debug.LogError("Too many emote layers!");

        for (int i = 0; i < emoteLayers.Length; i++)
        {
            for (int j = 0; j < emoteLayers[i].emotes.Length; j++)
            {
                emoteLayers[i].emotes[j].id = (i) | (j << 3);
            }
        }
    }

    public Emote? GetEmote(int id)
    {
        int layer = id & 0b111;
        int index = id >> 3;

        if (emoteLayers.Length < layer || emoteLayers[layer].emotes.Length < index)
            return null;

        return emoteLayers[layer].emotes[index];
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

}
