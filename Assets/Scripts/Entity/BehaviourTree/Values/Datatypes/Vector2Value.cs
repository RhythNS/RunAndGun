using UnityEngine;

namespace Rhyth.BTree
{
    public class Vector2Value : Value
    {
        [SerializeField] private Vector2 value;

        public override Value Clone()
        {
            Vector2Value vectorValue = CreateInstance<Vector2Value>();
            vectorValue.value = value;
            return vectorValue;
        }

        public override object GetValue() => value;

        public Vector2 Get() => value;

        public override void SetValue(object obj) => value = (Vector2)obj;

        public void Set(Vector2 vector) => value = vector;
    }
}
