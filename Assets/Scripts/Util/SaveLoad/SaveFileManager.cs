using UnityEngine;

public class SaveFileManager : MonoBehaviour
{
    [SerializeField] public SaveGame saveGame;

    private void Awake()
    {
        saveGame = Saver.Load();
    }

    public void Save()
    {
        Saver.Save(saveGame);
    }
}
