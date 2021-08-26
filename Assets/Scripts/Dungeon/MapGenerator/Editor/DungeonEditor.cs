using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom Editor for the DungeonCreator.
/// </summary>
[CustomEditor(typeof(DungeonCreator))]
public class DungeonEditor : Editor
{
    private DungeonCreator dungeonCreator;

    private void OnEnable() {
        dungeonCreator = (DungeonCreator)target;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (GUILayout.Button("Create new dungeon")) {
            dungeonCreator.CreateDungeon(int.MaxValue, 0, MapGenerator.DungeonConfig.StandardConfig, RegionDict.Instance.Tileset);
        }
    }
}
