using UnityEngine;

namespace Rhyth.BTree
{
    public class ObjectValue : Value
    {
        [SerializeField] private object value;

        public override Value Clone()
        {
            ObjectValue objectValue = CreateInstance<ObjectValue>();
            objectValue.value = value;
            return objectValue;
        }

        public override object GetValue() => value;

        public override void SetValue(object obj) => value = obj;
    }
}