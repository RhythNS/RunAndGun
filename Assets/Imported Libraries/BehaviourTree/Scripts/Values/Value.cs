using UnityEngine;

namespace Rhyth.BTree
{
    public abstract class Value : ScriptableObject
    {
        public abstract object GetValue();

        public virtual bool TryGetValue<T>(out T value)
        {
            object obj = GetValue();
            if (obj != null && obj is T t)
            {
                value = t;
                return true;
            }
            value = default;
            return false;
        }

        public abstract void SetValue(object obj);

        public abstract Value Clone();
    }
}
