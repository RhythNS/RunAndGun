using System.Collections.Generic;
using UnityEngine;

namespace Rhyth.BTree
{
    public class SubTreeNode : BNode
    {
        public override string StringToolTip => "Runs another Behaviour Tree as a node. Be careful not to cause an endless loop.\nReturns status of the ran Behaviour Tree on finish.";
        public override int MaxNumberOfChildren => 0;

        [SerializeField] private BTree behaviourTree;

        public BTree ClonedTree { get; private set; }

        [SerializeField] private ValueToOverwrite[] valuesToOverwrite;

        public override void InnerBeginn()
        {
            ClonedTree.AttachedBrain = tree.AttachedBrain;
            ClonedTree.Beginn();
        }

        public override void InnerRestart()
        {
            ClonedTree.Restart();
        }

        public override void Update()
        {
            switch (ClonedTree.Root.CurrentStatus)
            {
                case Status.Running:
                    ClonedTree.Update();
                    break;
                case Status.Success:
                    CurrentStatus = Status.Success;
                    break;
                case Status.Failure:
                    CurrentStatus = Status.Failure;
                    break;
            }
        }

        protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
        {
            SubTreeNode subTree = CreateInstance<SubTreeNode>();
            subTree.behaviourTree = behaviourTree;

            for (int i = 0; i < valuesToOverwrite.Length; i++)
            {
                if (valuesToOverwrite[i].newValue != null)
                {
                    Value replaceValue = CloneValue(originalValueForClonedValue, valuesToOverwrite[i].newValue);
                    originalValueForClonedValue.Add(valuesToOverwrite[i].toOverwrite, replaceValue);
                }
            }

            subTree.ClonedTree = behaviourTree.Clone(originalValueForClonedValue);

            return subTree;
        }

        protected override void InnerReplaceValues(Dictionary<Value, Value> originalReplace)
        {
            behaviourTree.ReplaceValues(originalReplace);
        }

        [System.Serializable]
        public struct ValueToOverwrite
        {
            public Value toOverwrite;
            public Value newValue;

            public ValueToOverwrite(Value toOverwrite, Value newValue)
            {
                this.toOverwrite = toOverwrite;
                this.newValue = newValue;
            }
        }
    }
}