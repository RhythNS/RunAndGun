using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class Saver
{
    public static void Save(SaveGame saveGame)
    {
        string toSaveTo = Application.persistentDataPath + "/save.game";

        using (FileStream fileStream = File.Exists(toSaveTo) == true ? File.OpenWrite(toSaveTo) : File.Create(toSaveTo))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fileStream, saveGame);
            fileStream.Close();
        }
    }

    public static SaveGame Load()
    {
        string toSaveTo = Application.persistentDataPath + "/save.game";

        if (File.Exists(toSaveTo) == false)
            return null;

        using (FileStream fileStream = File.OpenRead(toSaveTo))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            SaveGame toRet = formatter.Deserialize(fileStream) as SaveGame;
            fileStream.Close();
            return toRet;
        }
    }
}
