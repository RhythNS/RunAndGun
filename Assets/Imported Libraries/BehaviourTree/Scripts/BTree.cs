using System.Collections.Generic;
using UnityEngine;

namespace Rhyth.BTree
{
    [CreateAssetMenu(menuName = "BehaviourTree")]
    public class BTree : ScriptableObject
    {
        public Brain AttachedBrain { get; set; }

        [SerializeField] private BNode root;
        public BNode Root => root;

        public BNode.Status Status { get => root.CurrentStatus; }
        
        public void Setup() => root.Setup(this);

        public void Beginn() => root.Beginn();

        public void Update() => root.Update();

        public void Restart() => root.Restart();

        public BTree Clone()
        {
            Dictionary<Value, Value> originalValueForClonedValue = new Dictionary<Value, Value>();
            return Clone(originalValueForClonedValue);
        }

        public BTree Clone(Dictionary<Value, Value> originalValueForClonedValue)
        {
            BTree tree = CreateInstance<BTree>();
            tree.root = root.Clone(originalValueForClonedValue);
            return tree;
        }

        /// <summary>
        /// Replaces all key "Value" with the value "Value" of the dictionary.
        /// </summary>
        /// <param name="originalReplace">TKey is the target and TValue the replacement.</param>
        public void ReplaceValues(Dictionary<Value, Value> originalReplace)
        {
            root.ReplaceValues(originalReplace);
        }
    }
}