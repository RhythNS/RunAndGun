using UnityEngine;

namespace Rhyth.BTree
{
    public class IntValue : Value
    {
        [SerializeField] private int value;

        public override Value Clone()
        {
            IntValue intValue = CreateInstance<IntValue>();
            intValue.value = value;
            return intValue;
        }

        public override object GetValue() => value;

        public int Get() => value;

        public override void SetValue(object obj) => value = (int)obj;

        public void Set(int value) => this.value = value;
    }
}
