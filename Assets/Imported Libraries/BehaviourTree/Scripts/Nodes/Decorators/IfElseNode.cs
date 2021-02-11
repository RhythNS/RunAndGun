using System.Collections.Generic;
using UnityEngine;

namespace Rhyth.BTree
{
    public class IfElseNode : BNodeAdapter
    {
        public override int MaxNumberOfChildren => 3;

        public override string StringInEditor => "if / else";
        public override string StringToolTip => "This node needs exactly 3 children. The first node must be a BoolNode. The second node is the \"If\" node and the third node is the \"Else\" node." +
            "\nRuns the BoolNode every specified seconds. If the node changed it checks the ReturnOperation onNodeChanged. It then runs either the If node if the BoolNode return success or the Else node if the BoolNode returned failure.";

        [SerializeField] private float checkEverySeconds;
        [SerializeField] BNodeUtil.ReturnOperation onNodeChange = BNodeUtil.ReturnOperation.Restart;
        [SerializeField] BNodeUtil.ReturnOperation onIfNodeSuccess = BNodeUtil.ReturnOperation.ReturnSuccess;
        [SerializeField] BNodeUtil.ReturnOperation onIfNodeFailure = BNodeUtil.ReturnOperation.ReturnFailure;
        [SerializeField] BNodeUtil.ReturnOperation onElseNodeSuccess = BNodeUtil.ReturnOperation.ReturnSuccess;
        [SerializeField] BNodeUtil.ReturnOperation onElseNodeFailure = BNodeUtil.ReturnOperation.ReturnFailure;

        private float timer;

        private bool wasOnSecondChild = false;
        private bool started = false;
        private BoolNode firstChild;
        private BNode currentExecutingNode;

        public override void InnerRestart()
        {
            started = false;
            timer = -1;
            for (int i = 0; i < children.Length; i++)
                children[i].Restart();
        }

        public override void InnerSetup()
        {
            if (children.Length != 3)
            {
                Debug.LogError("IfElseNode in " + Brain.name + " does not have exaclty 3 children!");
                return;
            }

            if (children[0] is BoolNode boolNode)
                firstChild = boolNode;
            else
            {
                Debug.LogError("First child in IfElseNode in " + Brain.name + " is not a BoolNode!");
                return;
            }
        }

        public override void Update()
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                timer = checkEverySeconds;

                firstChild.Restart();
                firstChild.Beginn();
                bool useSecondChild = firstChild.IsFulfilled();

                if (!started)
                {
                    started = true;

                    SetExecutingNode(useSecondChild);
                }
                else if (wasOnSecondChild != useSecondChild)
                {

                    switch (onNodeChange)
                    {
                        case BNodeUtil.ReturnOperation.ReturnFailure:
                            CurrentStatus = Status.Failure;
                            return;
                        case BNodeUtil.ReturnOperation.ReturnSuccess:
                            CurrentStatus = Status.Success;
                            return;
                    }

                    SetExecutingNode(useSecondChild);
                }
            }

            switch (currentExecutingNode.CurrentStatus)
            {
                case Status.Running:
                    currentExecutingNode.Update();
                    break;
                case Status.Success:
                    OnCheckReturnOperation(wasOnSecondChild ? onIfNodeSuccess : onElseNodeSuccess, currentExecutingNode);
                    break;
                case Status.Failure:
                    OnCheckReturnOperation(wasOnSecondChild ? onIfNodeFailure : onElseNodeFailure, currentExecutingNode);
                    break;
            }
        }

        private void SetExecutingNode(bool useSecondChild)
        {
            currentExecutingNode = useSecondChild ? children[1] : children[2];

            currentExecutingNode.Restart();
            currentExecutingNode.Beginn();

            wasOnSecondChild = useSecondChild;
        }

        private bool OnCheckReturnOperation(BNodeUtil.ReturnOperation operation, BNode node)
        {
            switch (operation)
            {
                case BNodeUtil.ReturnOperation.ReturnFailure:
                    CurrentStatus = Status.Failure;
                    return false;
                case BNodeUtil.ReturnOperation.Restart:
                    node.Restart();
                    node.Beginn();
                    return true;
                case BNodeUtil.ReturnOperation.ReturnSuccess:
                    CurrentStatus = Status.Success;
                    return false;
                default:
                    throw new System.Exception(operation + " not implemented!");
            }
        }

        protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
        {
            IfElseNode ien = CreateInstance<IfElseNode>();
            ien.checkEverySeconds = checkEverySeconds;
            ien.onNodeChange = onNodeChange;
            ien.onIfNodeSuccess = onIfNodeSuccess;
            ien.onIfNodeFailure = onIfNodeFailure;
            ien.onElseNodeSuccess = onElseNodeSuccess;
            ien.onElseNodeFailure = onElseNodeFailure;
            return ien;
        }
    }
}