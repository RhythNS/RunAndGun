using UnityEngine;

/// <summary>
/// Dict for data about characters.
/// </summary>
public class CharacterDict : MonoBehaviour
{
    public static CharacterDict Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Already a CharacterDict in scene! Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    [System.Serializable]
    public struct CharacterTypeForPlayer
    {
        public Player player;
        public CharacterType characterType;
        public Sprite characterPreviewSprite;
    }

    [SerializeField] private CharacterTypeForPlayer[] players;

    /// <summary>
    /// Array of all player colors.
    /// </summary>
    public Color[] PlayerColors => playerColors;
    [SerializeField] private Color[] playerColors;

    /// <summary>
    /// Array of all players ligher color used for tinting white images to the players colors.
    /// </summary>
    public Color[] LightPlayerColors => lightPlayerColors;
    [SerializeField] private Color[] lightPlayerColors;

    /// <summary>
    /// Gets a player prefab for a given character type.
    /// </summary>
    /// <param name="characterType">The character type.</param>
    /// <returns>The prefab of the character.</returns>
    public Player GetPlayerForType(CharacterType characterType)
    {
        for (int i = 0; i < players.Length; i++)
            if (players[i].characterType == characterType)
                return players[i].player;

        Debug.LogError("Could not find " + characterType);
        return null;
    }

    /// <summary>
    /// Gets the sprite for a character type.
    /// </summary>
    /// <param name="characterType">The character type.</param>
    /// <returns>The sprite for the character type.</returns>
    public Sprite GetSpriteForType(CharacterType characterType)
    {
        for (int i = 0; i < players.Length; i++)
            if (players[i].characterType == characterType)
                return players[i].characterPreviewSprite;

        Debug.LogError("Could not find " + characterType);
        return null;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
