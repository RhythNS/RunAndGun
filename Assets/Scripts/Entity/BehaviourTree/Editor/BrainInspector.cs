using Rhyth.BTree;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom Inspector for the Brain
/// </summary>
[CustomEditor(typeof(Brain))]
public class BrainInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BTree tree = (BTree)serializedObject.FindProperty("tree").objectReferenceValue;

        // Open Tree button
        if (tree != null && GUILayout.Button("Open Tree"))
        {
            BTreeEditor.OpenWindow(tree);
        }
    }
}
