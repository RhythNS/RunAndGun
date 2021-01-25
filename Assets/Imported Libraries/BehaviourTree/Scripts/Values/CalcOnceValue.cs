using UnityEngine;

namespace Rhyth.BTree
{
    public class CalcOnceValue : Value
    {
        [SerializeField] private Value value;
        private object savedValue;
        private bool valueAlreadyCalced;

        public override Value Clone()
        {
            CalcOnceValue once = CreateInstance<CalcOnceValue>();
            once.value = value.Clone();
            return once;
        }

        public override object GetValue()
        {
            if (valueAlreadyCalced)
                return savedValue;
            valueAlreadyCalced = true;
            return savedValue = value.GetValue();
        }

        public override void SetValue(object obj)
        {
            value.SetValue(obj);
            savedValue = obj;
        }
    }
}