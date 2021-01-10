using System;
using UnityEditor;

public abstract class CustomBNodeEditor
{
    /// <summary>
    /// The Class for which the custom editor is for.
    /// </summary>
    public abstract Type NodeType { get; }

    /// <summary>
    /// Should the Editor draw all Properties it can find after the custom editor?
    /// </summary>
    public abstract bool AppendNormalEditor { get; }

    /// <summary>
    /// Called when the serilzied objects properties are supposed to be drawn.
    /// </summary>
    /// <param name="serializedObject">The Node as a SerializedObject</param>
    public abstract void DrawInspector(SerializedObject serializedObject);
}
