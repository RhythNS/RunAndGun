using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(Pickable), true)]
public class PickableEditor : Editor
{
    private static bool debugMode = true;

    public override void OnInspectorGUI()
    {
        Pickable pickable = target as Pickable;
        serializedObject.UpdateIfRequiredOrScript();
        SerializedProperty prop = serializedObject.GetIterator();
        while (prop.NextVisible(true))
        {
            if (prop.name == "id")
            {
                if (prop.intValue == 0 && GetNewId(pickable, out ushort id))
                    prop.intValue = id;

                // If we are not in debug mode then do not draw the id
                if (!debugMode)
                    continue;
            }

            EditorGUILayout.PropertyField(prop);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private bool GetNewId(Pickable pickable, out ushort id)
    {
        string[] guids = AssetDatabase.FindAssets(GetSearchString(pickable));
        List<Pickable> pickables = new List<Pickable>();
        for (int i = 0; i < guids.Length; i++)
        {
            pickables.Add(AssetDatabase.LoadAssetAtPath<Pickable>(AssetDatabase.GUIDToAssetPath(guids[i])));
        }

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

    private void FixIDs(List<Pickable> pickables)
    {
        for (int i = 0; i < pickables.Count; i++)
        {
            SerializedObject obj = new SerializedObject(pickables[i]);
            obj.FindProperty("id").intValue = i + 1;
            obj.ApplyModifiedProperties();
        }
    }

    private string GetSearchString(Pickable pickable)
    {
        switch (pickable.PickableType)
        {
            case PickableType.Consumable:
                return "t:Consumable";
            case PickableType.Item:
                return "t:Item";
            case PickableType.Weapon:
                return "t:Weapon";
        }
        throw new System.Exception("Could not find that type" + pickable.PickableType);
    }
}
