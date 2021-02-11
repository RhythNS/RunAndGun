using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rhyth.BTree
{
    public abstract class BNode : ScriptableObject
    {
        /// <summary>
        /// Used inside the NodeEditor. When the breakPoint is set to true, the game pauses
        /// when Beginn is called on this node.
        /// </summary>
        public bool BreakPointEnabled => breakPointEnabled;
        [SerializeField] private bool breakPointEnabled;

        /// <summary>
        /// Inilizing children at 0 because of Unity Serilization.
        /// </summary>
        public BNode() => children = new BNode[0];

        /// <summary>
        /// Only children that are of type or are derived of that type inside this array
        /// are allowed to be a child of this node.
        /// </summary>
        public virtual Type[] AllowedChildrenTypes { get; } = null;

        public enum Status
        {
            Waiting, Running, Success, Failure
        }

        /// <summary>
        /// The current Status of this node.
        /// Waiting: Has been restarted and is waiting for start.
        /// Running: Start has been called but the node is still executing.
        /// Success: Node has finished successfully.
        /// Failure: Node has finished unsuccessfully.
        /// </summary>
        public Status CurrentStatus { get; protected set; } = Status.Waiting;

        /// <summary>
        /// The max number of children that are expected. -1 is infinite amount of children.
        /// </summary>
        public abstract int MaxNumberOfChildren { get; }
        [SerializeField] protected BNode[] children;
        public BNode[] Children => children;

        /// <summary>
        /// These are the coordinates, width and height inside the editor.
        /// </summary>
        public Rect boundsInEditor;

        /// <summary>
        /// This is shown above the name in the editor. A symbol like string should be used for quick identification.
        /// I.e. Sequence = "->", Selector = "?"
        /// </summary>
        public virtual string StringInEditor { get => ""; }

        public virtual string StringToolTip { get => ""; }

        protected BTree tree;

        /// <summary>
        /// Shortcut to Brain.
        /// </summary>
        public Brain Brain => tree.AttachedBrain;

        /// <summary>
        /// Shortcut to BrainMover.
        /// </summary>
        public BrainMover Mover => tree.AttachedBrain.BrainMover;

        /// <summary>
        /// Called after the tree was loaded into memory. This is only called once per lifetime of this object.
        /// It should be used to set values that are not going to change during execution.
        /// </summary>
        /// <param name="tree">The tree that the node is on.</param>
        public virtual void Setup(BTree tree)
        {
            this.tree = tree;
            InnerSetup();

            for (int i = 0; i < children.Length; i++)
                children[i].Setup(tree);
        }

        /// <summary>
        /// Called after the tree was loaded into memory. This is only called once per lifetime of this object.
        /// It should be used to set values that are not going to change during execution.
        /// </summary>
        public abstract void InnerSetup();

        /// <summary>
        /// Called after Restart(). Should be used to get needed values and prep the node for execution.
        /// </summary>
        public void Beginn()
        {
            CurrentStatus = Status.Running;
            InnerBeginn();

#if UNITY_EDITOR // <-- Not sure if this is needed or not
            if (breakPointEnabled)
                Debug.Break();
#endif
        }

        /// <summary>
        /// Called after Restart(). Should be used to get needed values and prep the node for execution.
        /// </summary>
        public abstract void InnerBeginn();

        /// <summary>
        /// Called every frame. Should be used to update the node. When finished with execution use CurrentStatus
        /// to update the status.
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Should be used to remove any references and clean up from previous runs.
        /// </summary>
        public void Restart()
        {
            CurrentStatus = Status.Waiting;
            InnerRestart();
        }

        /// <summary>
        /// Should be used to remove any references and clean up from previous runs.
        /// </summary>
        public abstract void InnerRestart();

        /// <summary>
        /// Replaces all key "Value" with the value "Value" of the dictionary.
        /// </summary>
        /// <param name="originalReplace">TKey is the target and TValue the replacement.</param>
        public void ReplaceValues(Dictionary<Value, Value> originalReplace)
        {
            InnerReplaceValues(originalReplace);
            for (int i = 0; i < children.Length; i++)
                children[i].ReplaceValues(originalReplace);
        }

        /// <summary>
        /// Replaces all Values from this class with new values inside the dictionary.
        /// </summary>
        /// <param name="originalReplace">TKey is the target and TValue the replacement.</param>
        protected abstract void InnerReplaceValues(Dictionary<Value, Value> originalReplace);

        /// <summary>
        /// Clones the node and all its children.
        /// </summary>
        public BNode Clone(Dictionary<Value, Value> originalValueForClonedValue)
        {
            BNode[] children = new BNode[this.children.Length];
            for (int i = 0; i < children.Length; i++)
                children[i] = this.children[i].Clone(originalValueForClonedValue);

            BNode cloned = InnerClone(originalValueForClonedValue);
            cloned.children = children;
            cloned.boundsInEditor = boundsInEditor;
            cloned.breakPointEnabled = breakPointEnabled;
            cloned.name = name;
            return cloned;
        }

        /// <summary>
        /// Returns a cloned node with only class specific copied fields attached to the new clone.
        /// </summary>
        protected abstract BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue);

        /// <summary>
        /// Clones a value. Should be used when cloning a field which is a Value.
        /// </summary>
        protected Value CloneValue(Dictionary<Value, Value> originalValueForClonedValue, Value toClone)
        {
            if (originalValueForClonedValue.ContainsKey(toClone))
                return originalValueForClonedValue[toClone];
            Value cloned = toClone.Clone();
            originalValueForClonedValue.Add(toClone, cloned);
            return cloned;
        }
    }
}
