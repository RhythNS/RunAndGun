using System.Collections.Generic;
using UnityEngine;

namespace Rhyth.BTree
{
    /// <summary>
    /// Runs any number of children in parallel. Exits execution dependent on the
    /// OnFinish fileds for when a child fails or succeeds.
    /// </summary>
    public class ParallelNode : BNodeAdapter
    {
        public override string StringToolTip => "Runs any number of children in parallel. Return value is determined by the ReturnOperation values.";
        public override string StringInEditor => "| |";

        public override int MaxNumberOfChildren => -1;

        [SerializeField] private BNodeUtil.ReturnOperation childFailed;
        [SerializeField] private BNodeUtil.ReturnOperation childSucceeded;

        public override void InnerBeginn()
        {
            for (int i = 0; i < children.Length; i++)
            {
                children[i].Beginn();
            }
        }

        public override void InnerRestart()
        {
            for (int i = 0; i < children.Length; i++)
            {
                children[i].Restart();
            }
        }

        public override void Update()
        {
            for (int i = 0; i < children.Length; i++)
            {
                children[i].Update();
                if (children[i].CurrentStatus == Status.Failure)
                {
                    if (HandleOnFinish(childFailed, i) == false)
                        return;
                }
                else if (children[i].CurrentStatus == Status.Success)
                {
                    if (HandleOnFinish(childSucceeded, i) == false)
                        return;
                }
            }
        }

        /// <summary>
        /// Handles what happens when a child finishes execution.
        /// </summary>
        /// <param name="onFinish">The action that should be done.</param>
        /// <param name="childIndex">The child that finished execution.</param>
        /// <returns>Wheter the node should continue to execute its children.</returns>
        private bool HandleOnFinish(BNodeUtil.ReturnOperation onFinish, int childIndex)
        {
            switch (onFinish)
            {
                case BNodeUtil.ReturnOperation.ReturnFailure:
                    CurrentStatus = Status.Failure;
                    return false;
                case BNodeUtil.ReturnOperation.Restart:
                    children[childIndex].Restart();
                    children[childIndex].Beginn();
                    break;
                case BNodeUtil.ReturnOperation.ReturnSuccess:
                    CurrentStatus = Status.Success;
                    return false;
            }
            return true;
        }

        protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
        {
            ParallelNode node = CreateInstance<ParallelNode>();
            node.childFailed = childFailed;
            node.childSucceeded = childSucceeded;
            return node;
        }
    }
}
