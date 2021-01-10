using Rhyth.BTree;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SubTreeEditor : CustomBNodeEditor
{
    public override Type NodeType => typeof(SubTreeNode);

    public override bool AppendNormalEditor => false;

    public override void DrawInspector(SerializedObject serializedObject)
    {
        SubTreeNode objectReference = serializedObject.targetObject as SubTreeNode;
        SerializedProperty valuesToOverwrite = serializedObject.FindProperty("valuesToOverwrite");
        SerializedProperty treeProperty = serializedObject.FindProperty("behaviourTree");

        EditorGUILayout.PropertyField(treeProperty);

        if (GUILayout.Button("Open Tree"))
        {
            BTreeEditor editor = ScriptableObject.CreateInstance<BTreeEditor>();
            editor.OpenSubTree(treeProperty.objectReferenceValue as BTree, objectReference.ClonedTree);
            editor.Show();
        }

        if (GUILayout.Button("Update ValueList"))
        {
            if (treeProperty.objectReferenceValue == null)
                goto DrawArrayValues; // skip the button

            UnityEngine.Object[] allObjects = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(treeProperty.objectReferenceValue));

            List<Value> values = new List<Value>();

            for (int i = 0; i < allObjects.Length; i++)
                if (allObjects[i] is Value)
                    values.Add(allObjects[i] as Value);

            for (int i = valuesToOverwrite.arraySize - 1; i > -1; i--)
            {
                // Get the element "valuesToOverwrite.toOverwrite"
                SerializedProperty element = valuesToOverwrite.GetArrayElementAtIndex(i).FindPropertyRelative("toOverwrite");

                // Go through both lists and remove all matching elements and remove elements which are not in allnames from the
                // serialized array
                bool found = false;
                for (int j = 0; j < values.Count; j++)
                {
                    if (values[j] == element.objectReferenceValue)
                    {
                        values.RemoveAt(j);
                        found = true;
                        break;
                    }
                }
                if (found == false)
                {
                    SerializableUtil.ArrayRemoveAtIndex(valuesToOverwrite, i);
                }
            }

            // save the index and grow the array
            int index = valuesToOverwrite.arraySize;
            valuesToOverwrite.arraySize += values.Count;

            for (int i = 0; i < values.Count; i++)
                valuesToOverwrite.GetArrayElementAtIndex(index++).FindPropertyRelative("toOverwrite").objectReferenceValue = values[i];

            serializedObject.ApplyModifiedProperties();
        }

    DrawArrayValues:
        // Draw the array
        for (int i = 0; i < valuesToOverwrite.arraySize; i++)
        {
            SerializedProperty element = valuesToOverwrite.GetArrayElementAtIndex(i);
            SerializedProperty toOverwrite = element.FindPropertyRelative("toOverwrite");
            SerializedProperty newValue = element.FindPropertyRelative("newValue");

            newValue.objectReferenceValue = EditorGUILayout.ObjectField(toOverwrite.objectReferenceValue.name, newValue.objectReferenceValue, typeof(Value), false);
        }

    }

}