using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
            dungeonCreator.CreateDungeon(int.MaxValue, 0, new MapGenerator.DungeonConfig());
        }
    }
}
