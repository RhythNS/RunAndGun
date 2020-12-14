using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TiledSharp;
using System.IO;

public class TestingMap : MonoBehaviour
{
    public Tilemap map;
    public string location;
    public string mapName;
    private List<Texture2D> texs = new List<Texture2D>();


    private void Start()
    {
        TextAsset mapString = Resources.Load(location + "/" + mapName) as TextAsset;
        MemoryStream stream = new MemoryStream(mapString.bytes);
        TmxMap map = new TmxMap(stream);
        for (int i = 0; i < map.Tilesets.Count; i++)
        {
            map.Tilesets[i].Load(location);
        }


        stream.Close();

        for (int j = 0; j < 1000; j++)
        {
            for (int i = 0; i < map.Tilesets.Count; i++)
            {
                string image = map.Tilesets[i].Image.Source;
                image = image.Substring(0, image.Length - 4);
                texs.Add(Resources.Load<Texture2D>(location + "/" + image));
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            for (int i = 0; i < texs.Count - 1; i++)
            {
                for (int j = i + 1; j < texs.Count; j++)
                {
                    if (texs[i].GetHashCode() != texs[j].GetHashCode())
                    {
                        Debug.LogError("Welp");
                    }
                }
            }
        }
    }

}
