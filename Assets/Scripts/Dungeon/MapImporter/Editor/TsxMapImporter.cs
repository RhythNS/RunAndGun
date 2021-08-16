using System.IO;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

/// <summary>
/// Custom importer for importing tsx files.
/// </summary>
[ScriptedImporter(1, "tsx")]
public class TsxMapImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        TextAsset subAsset = new TextAsset(File.ReadAllText(ctx.assetPath));
        ctx.AddObjectToAsset("text", subAsset);
        ctx.SetMainObject(subAsset);
    }
}
