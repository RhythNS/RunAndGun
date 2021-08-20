using System.IO;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

/// <summary>
/// Custom importer for importing tmx files.
/// </summary>
[ScriptedImporter(1, "tmx")]
public class TmxMapImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        TextAsset subAsset = new TextAsset(File.ReadAllText(ctx.assetPath));
        ctx.AddObjectToAsset("text", subAsset);
        ctx.SetMainObject(subAsset);
    }
}
