using System.Collections.Generic;
using UnityEngine;

namespace Rhyth.BTree
{
    /// <summary>
    /// Variation of the Guard node. Repeats the second child until either the first child or the second child fails.
    /// What happens when either node failes is determined by the ReturnOperation enum.
    /// </summary>
    public class RepeatUntilFailure : BNodeAdapter
    {
        public override string StringToolTip => "Repeates the second child until the first child or the second child returns failure.\nWhat happens when either node failes is determined by the ReturnOperation.";

        public override string StringInEditor => "↻ until Failure";

        public override int MaxNumberOfChildren => 2;

        [SerializeField] private BNodeUtil.Check check;
        [SerializeField] private float checkEverySeconds;
        [SerializeField] private BNodeUtil.ReturnOperation child1OnFailure;
        [SerializeField] private BNodeUtil.ReturnOperation child2OnFailure;

        private float timer;
        private bool restartedFirstChild;
        private bool forceCheck;

        private BoolNode boolNode;

        private bool displayedWarning = false;

        public override void InnerBeginn()
        {
            if (children.Length == 2)
            {
                if (displayedWarning == false &&
                    child1OnFailure == BNodeUtil.ReturnOperation.Restart &&
                    child2OnFailure == BNodeUtil.ReturnOperation.Restart)
                {
                    Debug.LogWarning("The node " + name + " on " + tree.AttachedBrain.name + 
                        " is configured to be in an endless loop");
                    displayedWarning = true;
                }

                if (children[0] is BoolNode)
                    boolNode = children[0] as BoolNode;

                children[1].Beginn(tree);
            }
        }

        public override void InnerRestart()
        {
            timer = checkEverySeconds;
            restartedFirstChild = false;
            if (children.Length == 2)
            {
                children[1].Restart();
            }
        }

        public override void Update()
        {
            if (children.Length != 2)
            {
                Debug.LogWarning("Node " + name + " on " + tree.AttachedBrain.name + 
                    " has not 2 children. It will always return failure!");
                CurrentStatus = Status.Failure;
            }

            if (check == BNodeUtil.Check.EverySeconds || forceCheck == true)
            {
                timer -= Time.deltaTime;
                if (timer < 0 || forceCheck == true)
                {
                    // If the firstChild is a bool node
                    if (boolNode != null)
                    {
                        boolNode.Restart();
                        boolNode.Beginn(tree);

                        // if boolNode failed
                        if (boolNode.IsFulfilled() != true)
                        {
                            if (OnChild1Failure() == false)
                                return;
                        }

                        timer = checkEverySeconds;
                        forceCheck = false;
                    }
                    else
                    {
                        // first child is an ordinary node
                        // first check if we already restarted
                        if (restartedFirstChild == false)
                        {
                            children[0].Restart();
                            children[0].Beginn(tree);
                            restartedFirstChild = true;
                        }

                        switch (children[0].CurrentStatus)
                        {
                            case Status.Running:
                                children[0].Update();
                                break;
                            case Status.Success: // Restart the node
                                timer = checkEverySeconds;
                                restartedFirstChild = false;
                                forceCheck = false;
                                break;
                            case Status.Failure:
                                if (OnChild1Failure() == true)
                                    goto case Status.Success;
                                return;
                        }
                    }
                } // end if timer < 0
            } // end if check is everyseconds

            switch (children[1].CurrentStatus)
            {
                case Status.Running:
                    children[1].Update();
                    break;
                case Status.Success: // restart the node
                    children[1].InnerRestart();
                    children[1].Beginn(tree);
                    forceCheck = true;
                    break;
                case Status.Failure:
                    switch (child2OnFailure)
                    {
                        case BNodeUtil.ReturnOperation.ReturnFailure:
                            CurrentStatus = Status.Failure;
                            return;
                        case BNodeUtil.ReturnOperation.ReturnSuccess:
                            CurrentStatus = Status.Success;
                            return;
                    }
                    // case was restarting so jump to success case i.e. restart the node
                    goto case Status.Success;
            }

        }

        /// <summary>
        /// Handles a failure on child1. Returns wheter to continue execution
        /// </summary>
        private bool OnChild1Failure()
        {
            switch (child1OnFailure)
            {
                case BNodeUtil.ReturnOperation.ReturnFailure:
                    CurrentStatus = Status.Failure;
                    return false;
                case BNodeUtil.ReturnOperation.Restart:
                    return true;
                case BNodeUtil.ReturnOperation.ReturnSuccess:
                    CurrentStatus = Status.Success;
                    return false;
                default:
                    throw new System.Exception("Impossible to get here. Unimplemented case: " + child1OnFailure);
            }
        }

        protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
        {
            RepeatUntilFailure repeatUntilFailure = CreateInstance<RepeatUntilFailure>();
            repeatUntilFailure.check = check;
            repeatUntilFailure.checkEverySeconds = checkEverySeconds;
            repeatUntilFailure.child1OnFailure = child1OnFailure;
            repeatUntilFailure.child2OnFailure = child2OnFailure;
            return repeatUntilFailure;
        }
    }
}
