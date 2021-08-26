using Mirror;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

/// <summary>
/// Editor for the RAGNetworkManager. Some code was taken from the NetworkManagerEditor.
/// </summary>
[CustomEditor(typeof(RAGNetworkManager), false)]
[CanEditMultipleObjects]
public class RagNetworkManagerEditor : Editor
{
    SerializedProperty spawnListProperty;
    ReorderableList spawnList;
    protected NetworkManager networkManager;

    protected void Init()
    {
        if (spawnList == null)
        {
            networkManager = target as NetworkManager;
            spawnListProperty = serializedObject.FindProperty("spawnPrefabs");
            spawnList = new ReorderableList(serializedObject, spawnListProperty)
            {
                drawHeaderCallback = DrawHeader,
                drawElementCallback = DrawChild,
                onReorderCallback = Changed,
                onRemoveCallback = RemoveButton,
                onChangedCallback = Changed,
                onAddCallback = AddButton,
                // this uses a 16x16 icon. other sizes make it stretch.
                elementHeight = 16
            };
        }
    }

    public override void OnInspectorGUI()
    {
        Init();
        DrawDefaultInspector();
        EditorGUI.BeginChangeCheck();
        spawnList.DoLayoutList();
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }

        if (GUILayout.Button("Auto Add Prefabs"))
        {
            SerializedProperty spawnListProperty = serializedObject.FindProperty("spawnPrefabs");


            string[] guids = AssetDatabase.FindAssets("t:prefab", new[] { "Assets/Prefabs/Network" });

            spawnListProperty.arraySize = guids.Length;
            for (int i = 0; i < guids.Length; i++)
            {
                spawnListProperty.GetArrayElementAtIndex(i).objectReferenceValue = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guids[i]));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

    static void DrawHeader(Rect headerRect)
    {
        GUI.Label(headerRect, "Registered Spawnable Prefabs:");
    }

    internal void DrawChild(Rect r, int index, bool isActive, bool isFocused)
    {
        SerializedProperty prefab = spawnListProperty.GetArrayElementAtIndex(index);
        GameObject go = (GameObject)prefab.objectReferenceValue;

        GUIContent label;
        if (go == null)
        {
            label = new GUIContent("Empty", "Drag a prefab with a NetworkIdentity here");
        }
        else
        {
            NetworkIdentity identity = go.GetComponent<NetworkIdentity>();
            label = new GUIContent(go.name, identity != null ? "AssetId: [" + identity.assetId + "]" : "No Network Identity");
        }

        GameObject newGameObject = (GameObject)EditorGUI.ObjectField(r, label, go, typeof(GameObject), false);

        if (newGameObject != go)
        {
            if (newGameObject != null && !newGameObject.GetComponent<NetworkIdentity>())
            {
                Debug.LogError("Prefab " + newGameObject + " cannot be added as spawnable as it doesn't have a NetworkIdentity.");
                return;
            }
            prefab.objectReferenceValue = newGameObject;
        }
    }

    internal void Changed(ReorderableList list)
    {
        EditorUtility.SetDirty(target);
    }

    internal void AddButton(ReorderableList list)
    {
        spawnListProperty.arraySize += 1;
        list.index = spawnListProperty.arraySize - 1;

        SerializedProperty obj = spawnListProperty.GetArrayElementAtIndex(spawnListProperty.arraySize - 1);
        obj.objectReferenceValue = null;

        spawnList.index = spawnList.count - 1;

        Changed(list);
    }

    internal void RemoveButton(ReorderableList list)
    {
        spawnListProperty.DeleteArrayElementAtIndex(spawnList.index);
        if (list.index >= spawnListProperty.arraySize)
        {
            list.index = spawnListProperty.arraySize - 1;
        }
    }
}
