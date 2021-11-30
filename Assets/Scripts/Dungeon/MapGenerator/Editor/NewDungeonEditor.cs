using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom Editor for the DungeonCreator.
/// </summary>
[CustomEditor(typeof(Assets.Scripts.Dungeon.MapGenRework.DungeonCreator))]
public class NewDungeonEditor : Editor
{
    private Assets.Scripts.Dungeon.MapGenRework.DungeonCreator dungeonCreator;

    private void OnEnable() {
        dungeonCreator = (Assets.Scripts.Dungeon.MapGenRework.DungeonCreator)target;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (GUILayout.Button("Create new dungeon")) {
            dungeonCreator.CreateDungeon();
        }
    }
}
