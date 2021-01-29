using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

public class PathValue : Value
{
    [SerializeField] private List<Vector2> path;

    public override Value Clone()
    {
        PathValue pv = CreateInstance<PathValue>();
        pv.path = path;
        return pv;
    }

    public List<Vector2> Get => path;

    public override object GetValue() => path;

    public override void SetValue(object obj) => path = obj as List<Vector2>;

    public void Set(List<Vector2> path) => this.path = path;
}
