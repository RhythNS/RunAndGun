using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Rhyth.BTree
{
    public partial class BTreeEditor : EditorWindow
    {
        /// <summary>
        /// Draws the left side of the window. This side is for displaying debug window values, what the root node is
        /// and handles the adding, selction and removing of Values.
        /// </summary>
        private void DrawTreeInfo()
        {
            if (DEBUG)
            {
                offset = EditorGUILayout.Vector2Field("Position:", offset);
                zoomLevel = Mathf.Clamp(EditorGUILayout.FloatField("Zoomlevel", zoomLevel), MIN_ZOOM_LEVEL, MAX_ZOOM_LEVEL);
            }

            lockTreeView = EditorGUILayout.Toggle("Lock Window to current Tree", lockTreeView);

            if (inPlayMode == false)
            {
                // Handle root node
                List<BNode> parentLessNodes = GetParentlessNodes(); // only nodes without parents can become root
                SerializedProperty rootNode = tree.FindProperty("root");
                BNode previousRoot = rootNode.objectReferenceValue as BNode;

                string[] choices = new string[parentLessNodes.Count + 1];
                choices[0] = "Null"; // If the root node should be set to nothing

                // fill all the choices with names of the nodes
                int index = 0;
                for (int i = 0; i < parentLessNodes.Count; i++)
                {
                    choices[i + 1] = parentLessNodes[i].name;
                    if (parentLessNodes[i] == previousRoot)
                        index = i + 1;
                }

                // Update the new root node
                Color preColor = GUI.color;
                if (index == 0)
                    GUI.color = Color.red;
                int newIndex = EditorGUILayout.Popup("root", index, choices);
                GUI.color = preColor;

                if (newIndex != index)
                {
                    if (newIndex == 0)
                        rootNode.objectReferenceValue = null;
                    else
                        rootNode.objectReferenceValue = parentLessNodes[newIndex - 1];
                }
                this.rootNode = newIndex == 0 ? null : parentLessNodes[newIndex - 1];
                
                // ---Handle root node

                tree.ApplyModifiedProperties();

                // Add the Button for adding a new value
                if (GUILayout.Button("Add Value"))
                {
                    GenericMenu valueMenu = new GenericMenu();
                    for (int i = 0; i < valueTypes.Length; i++)
                    {
                        Type type = valueTypes[i];
                        string name = valueTypes[i].Name;
                        if (name.EndsWith("value", StringComparison.OrdinalIgnoreCase))
                            name = name.Substring(0, name.Length - 5);

                        valueMenu.AddItem(new GUIContent(name), false, () =>
                        {
                            Value newNode = (Value)CreateInstance(type);
                            newNode.name = "New " + type.Name;
                            AssetDatabase.AddObjectToAsset(newNode, tree.targetObject);
                            selectedObject = newNode;
                            AssetDatabase.SaveAssets();
                            Reload();
                        });
                    }
                    valueMenu.ShowAsContext();
                }
            }

            Color prevColor = GUI.contentColor;
            // Display all values in a list
            for (int i = 0; i < allValues.Length; i++)
            {
                if (allValues[i] == selectedObject)
                    GUI.contentColor = currentSelectedColor;
                if (GUILayout.Button(allValues[i].name))
                {
                    selectedObject = allValues[i];
                }
                GUI.contentColor = prevColor;
            }
        }

        /// <summary>
        /// Properties that are ignored if debug is set to false.
        /// </summary>
        private static readonly string[] DEBUG_PROPERTIES = { "children", "boundsInEditor" };

        /// <summary>
        /// Properties that are ignored when triying to find every SerializedProperty.
        /// </summary>
        private static readonly string[] IGNORE_PROPERTIES = { "m_Script", "breakPointEnabled" };

        /// <summary>
        /// This draws the left side of the window. This side shows information about the currently selected Object
        /// which can be edited.
        /// </summary>
        private void DrawInspector()
        {
            if (selectedObject != null)
            {
                // Get the SerializedObject and draw every property
                SerializedObject serObject = new SerializedObject(selectedObject);
                SerializedProperty prop = serObject.GetIterator();
                if (prop.NextVisible(true))
                {
                    bool appendNormalProperties = true;
                    EditorGUILayout.LabelField(serObject.targetObject.GetType().Name);
                    serObject.targetObject.name = EditorGUILayout.TextField("Name", serObject.targetObject.name);

                    if (selectedObject is BNode)
                    {
                        SerializedProperty breakPointProperty = serObject.FindProperty("breakPointEnabled");
                        breakPointProperty.boolValue = EditorGUILayout.Toggle("Break Point Enabled", breakPointProperty.boolValue);

                        // If it has a customEditor, draw this before drawing the properties
                        if (typeForCustomEditor.TryGetValue(serObject.targetObject.GetType(), out CustomBNodeEditor editor))
                        {
                            editor.DrawInspector(serObject);
                            appendNormalProperties = editor.AppendNormalEditor;
                        }
                    }

                    if (appendNormalProperties == true)
                    {
                        do
                        {
                            DrawProperty(serObject, prop);
                        }
                        while (prop.NextVisible(false));
                    }
                }
                if (inPlayMode == false) // We should not be able to change anything in play mode
                    serObject.ApplyModifiedProperties();
            }

            // Draw the delete button
            if (inPlayMode == false)
            {
                bool changed = false;
                if (selectedObject is Value)
                {
                    if (GUILayout.Button("Delete"))
                    {
                        AssetDatabase.RemoveObjectFromAsset(selectedObject);
                        AssetDatabase.SaveAssets();
                        selectedObject = null;
                        changed = true;
                    }
                }
                else if (selectedObject is BNode)
                {
                    if (GUILayout.Button("Delete"))
                    {
                        RemoveNode(selectedObject as BNode);
                        changed = true;
                    }
                }
                if (changed)
                {
                    tree.ApplyModifiedProperties();
                    tree.Update();
                    Reload();
                }
            }
        }

        /// <summary>
        /// Draws a single property.
        /// </summary>
        /// <param name="serObject">The object where the property is from.</param>
        /// <param name="prop">The property to be drawn</param>
        private void DrawProperty(SerializedObject serObject, SerializedProperty prop)
        {
            for (int i = 0; i < IGNORE_PROPERTIES.Length; i++)
                if (prop.name == IGNORE_PROPERTIES[i])
                    return;

            for (int i = 0; i < DEBUG_PROPERTIES.Length && DEBUG == false; i++)
                if (prop.name == DEBUG_PROPERTIES[i])
                    return;

            Type type = GetTypeOfProperty(prop);
            if (typeof(Value).IsAssignableFrom(type))
            {
                List<string> allValuesStrings = new List<string>
                    {
                        "Null"
                    };

                int selected = 0;

                if (prop.objectReferenceValue != null)
                    for (int i = 0; i < allValues.Length; i++)
                    {
                        if (!allValues[i].GetType().IsAssignableFrom(type))
                            continue;

                        allValuesStrings.Add(allValues[i].name);

                        if (allValues[i] == prop.objectReferenceValue)
                            selected = allValuesStrings.Count - 1;
                    }

                int newSelected = EditorGUILayout.Popup(prop.name, selected, allValuesStrings.ToArray());

                if (selected != newSelected)
                {
                    prop.objectReferenceValue = newSelected == 0 ? null : (UnityEngine.Object)allValues[newSelected - 1];
                }

                return;
            }

            EditorGUILayout.PropertyField(serObject.FindProperty(prop.name), true);
        }

    }
}