using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a path that the entity can walk on.
/// </summary>
public class PathValue : Value
{
    [SerializeField] private List<Vector2> path;

    public override Value Clone()
    {
        PathValue pv = CreateInstance<PathValue>();
        pv.path = path;
        return pv;
    }

    /// <summary>
    /// Gets the raw array of the path.
    /// </summary>
    public List<Vector2> Get() => path;

    public override object GetValue() => path;

    public override void SetValue(object obj) => path = obj as List<Vector2>;

    /// <summary>
    /// Sets the path with an array.
    /// </summary>
    public void Set(List<Vector2> path) => this.path = path;

    /// <summary>
    /// Checks if a path has the same steps as this path.
    /// </summary>
    /// <param name="otherPath">The other path.</param>
    /// <returns>Wheter it is the same or not.</returns>
    public bool IsSame(PathValue otherPath) => IsSame(otherPath.Get());

    /// <summary>
    /// Checks if a path has the same steps as this path.
    /// </summary>
    /// <param name="otherPath">The other path.</param>
    /// <returns>Wheter it is the same or not.</returns>
    public bool IsSame(List<Vector2> otherPath)
    {
        if (otherPath == path)
            return true;

        if (otherPath == null || path == null || otherPath.Count != path.Count)
            return false;

        for (int i = 0; i < path.Count; i++)
        {
            if (path[i].Equals(otherPath[i]) == false)
                return false;
        }

        return true;
    }
}
