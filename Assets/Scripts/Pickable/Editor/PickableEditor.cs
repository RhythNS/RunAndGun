using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor of an pickable to add helper buttons.
/// </summary>
[CustomEditor(typeof(Pickable), true)]
public class PickableEditor : Editor
{
    private static readonly bool debugMode = false;

    public override void OnInspectorGUI()
    {
        Pickable pickable = target as Pickable;
        serializedObject.UpdateIfRequiredOrScript();
        SerializedProperty prop = serializedObject.GetIterator();

        if (GUILayout.Button("Fix IDs"))
            FixIDsOfType(pickable.PickableType);

        while (prop.NextVisible(true))
        {
            if (prop.name == "id")
            {
                if (prop.intValue == 0 && GetNewId(pickable, out ushort id))
                    prop.intValue = id;

                // If we are not in debug mode then do not draw the id
                if (!debugMode)
                {
                    EditorGUILayout.LabelField("Id", prop.intValue.ToString());
                    continue;
                }
            }

            EditorGUILayout.PropertyField(prop);
        }

        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// Fixes all ides.
    /// </summary>
    public static void FixAllIDs()
    {
        foreach (PickableType pickableType in Enum.GetValues(typeof(PickableType)))
            FixIDsOfType(pickableType);
    }

    /// <summary>
    /// Fixes ids of a particular pickable type.
    /// </summary>
    /// <param name="pickableType">The type to be fixed.s</param>
    public static void FixIDsOfType(PickableType pickableType) => FixIDs(LoadPickables(pickableType));

    /// <summary>
    /// Gets a new id that is currently not in use.
    /// </summary>
    /// <param name="pickable">The pickable to which the new id should be generated for.</param>
    /// <param name="id">The new id that should be used.</param>
    /// <returns>Wheter it successeded or not.</returns>
    private static bool GetNewId(Pickable pickable, out ushort id)
    {
        List<Pickable> pickables = LoadPickables(pickable.PickableType);

        // No other pickables of this type
        if (pickables.Count == 0)
        {
            id = 1;
            return true;
        }

        // Sort them from lowest to highest
        pickables.Sort((a, b) => a.Id.CompareTo(b.Id));

        // Check if there is a gap that we can fill
        for (int i = 1; i < pickables.Count; i++)
        {
            // IDs are somehow corrupted. Fix them.
            if (pickables[i].Id == pickables[i - 1].Id)
            {
                FixIDs(pickables);
                id = 0;
                return false;
            }

            // is the current id more than 1 different that the next id
            if (pickables[i].Id - pickables[i - 1].Id > 1)
            {
                id = (ushort)(pickables[i - 1].Id + 1);
                return true;
            }
        }

        // there were no gaps
        id = (ushort)(pickables[pickables.Count - 1].Id + 1);
        return true;
    }

    /// <summary>
    /// Loads all pickables of a type into a list.
    /// </summary>
    /// <param name="pickable">The type of pickable.</param>
    public static List<Pickable> LoadPickables(PickableType pickable)
    {
        string[] guids = AssetDatabase.FindAssets(GetSearchString(pickable));
        List<Pickable> pickables = new List<Pickable>();
        for (int i = 0; i < guids.Length; i++)
        {
            pickables.Add(AssetDatabase.LoadAssetAtPath<Pickable>(AssetDatabase.GUIDToAssetPath(guids[i])));
        }
        return pickables;
    }

    /// <summary>
    /// Gets all pickables and assignes them new ids.
    /// </summary>
    /// <param name="pickables">The pickables which ids needs fixing.</param>
    private static void FixIDs(List<Pickable> pickables)
    {
        for (int i = 0; i < pickables.Count; i++)
        {
            SerializedObject obj = new SerializedObject(pickables[i]);
            obj.FindProperty("id").intValue = i + 1;
            obj.ApplyModifiedProperties();
        }
    }

    public static string GetSearchString(PickableType pickable)
    {
        switch (pickable)
        {
            case PickableType.Consumable:
                return "t:Consumable";
            case PickableType.Item:
                return "t:Item";
            case PickableType.Weapon:
                return "t:Weapon";
            case PickableType.StatusEffect:
                return "t:StatusEffect";
            case PickableType.WorldEffect:
                return "t:WorldEffect";
        }
        throw new System.Exception("Could not find that type" + pickable);
    }
}
