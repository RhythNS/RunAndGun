using Mirror;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RAGNetworkManager), false)]
public class RagNetworkManagerEditor : NetworkManagerEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

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
}
