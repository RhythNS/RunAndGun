using UnityEngine;

namespace Rhyth.BTree
{
    public class VectorValue : Value
    {
        [SerializeField] private Vector3 value;

        public override Value Clone()
        {
            VectorValue vectorValue = CreateInstance<VectorValue>();
            vectorValue.value = value;
            return vectorValue;
        }

        public override object GetValue() => value;

        public Vector3 Get() => value;

        public override void SetValue(object obj) => value = (Vector3)obj;

        public void Set(Vector3 vector) => value = vector;

    }
}
