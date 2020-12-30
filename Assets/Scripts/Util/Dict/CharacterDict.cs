using UnityEngine;

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
    }

    [SerializeField] private CharacterTypeForPlayer[] players;

    public Player GetPlayerForType(CharacterType characterType)
    {
        for (int i = 0; i < players.Length; i++)
            if (players[i].characterType == characterType)
                return players[i].player;

        Debug.LogError("Could not find " + characterType);
        return null;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
