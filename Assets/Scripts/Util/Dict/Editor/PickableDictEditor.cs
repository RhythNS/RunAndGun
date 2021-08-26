using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom editor for the pickable dict in order to add all pickables automaticly into the array.
/// </summary>
[CustomEditor(typeof(PickableDict))]
public class PickableDictEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Auto add pickables"))
        {
            SerializedProperty prop = serializedObject.GetIterator();
            while (prop.NextVisible(true))
            {
                switch (prop.name)
                {
                    case "weapons":
                        AddAll(prop, PickableEditor.LoadPickables(PickableType.Weapon));
                        break;
                    case "consumables":
                        AddAll(prop, PickableEditor.LoadPickables(PickableType.Consumable));
                        break;
                    case "items":
                        AddAll(prop, PickableEditor.LoadPickables(PickableType.Item));
                        break;
                    case "statusEffects":
                        AddAll(prop, PickableEditor.LoadPickables(PickableType.StatusEffect));
                        break;
                    case "worldEffects":
                        AddAll(prop, PickableEditor.LoadPickables(PickableType.WorldEffect));
                        break;
                    default:
                        break;
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        if (GUILayout.Button("Fix all IDs"))
            PickableEditor.FixAllIDs();

        base.OnInspectorGUI();
    }

    private void AddAll(SerializedProperty prop, List<Pickable> pickables)
    {
        prop.arraySize = pickables.Count;
        for (int i = 0; i < pickables.Count; i++)
        {
            prop.GetArrayElementAtIndex(i).objectReferenceValue = pickables[i];
        }
    }
}
