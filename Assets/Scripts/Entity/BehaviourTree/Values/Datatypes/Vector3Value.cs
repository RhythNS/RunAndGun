using UnityEngine;

namespace Rhyth.BTree
{
    public class Vector3Value : Value
    {
        [SerializeField] private Vector3 value;

        public override Value Clone()
        {
            Vector3Value vectorValue = CreateInstance<Vector3Value>();
            vectorValue.value = value;
            return vectorValue;
        }

        public override object GetValue() => value;

        public Vector3 Get() => value;

        public override void SetValue(object obj) => value = (Vector3)obj;

        public void Set(Vector3 vector) => value = vector;
    }
}
