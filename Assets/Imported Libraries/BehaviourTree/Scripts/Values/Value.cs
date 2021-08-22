using UnityEngine;

namespace Rhyth.BTree
{
    /// <summary>
    /// Represents a variable in a behaviour tree.
    /// </summary>
    public abstract class Value : ScriptableObject
    {
        /// <summary>
        /// Gets the value of the variable.
        /// </summary>
        public abstract object GetValue();

        /// <summary>
        /// Try and cast the value to T.
        /// </summary>
        /// <typeparam name="T">The type to cast the value to.</typeparam>
        /// <param name="value">The gotten value. Is set to default value if the cast was unsuccessful.</param>
        /// <returns>Wheter the cast was successful.</returns>
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

        /// <summary>
        /// Sets the value of the variable.
        /// </summary>
        public abstract void SetValue(object obj);

        /// <summary>
        /// Clones the variable.
        /// </summary>
        public abstract Value Clone();
    }
}
