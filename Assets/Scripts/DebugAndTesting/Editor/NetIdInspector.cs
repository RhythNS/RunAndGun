using Mirror;
using UnityEditor;

[CustomEditor(typeof(NetworkIdentity))]
public class NetIdInspector : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();
        SerializedProperty prop = serializedObject.GetIterator();

        while (prop.NextVisible(true))
        {

            EditorGUILayout.PropertyField(prop);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
