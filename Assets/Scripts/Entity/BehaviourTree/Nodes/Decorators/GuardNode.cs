using System.Collections.Generic;
using UnityEngine;

namespace Rhyth.BTree
{
    /// <summary>
    /// Runs the second node until the first node fails.
    /// </summary>
    public class GuardNode : BNodeAdapter
    {
        public override string StringToolTip => "Runs the second node until the first node fails.\nReturns success if the second node returns success.\nReturns failure if the first or second node failed.";

        [SerializeField] private float checkEverySeconds = 1f;
        private float timer;

        public override int MaxNumberOfChildren => 2;

        public override void InnerBeginn()
        {
            for (int i = 0; i < children.Length; i++)
                children[i].Beginn(tree);
        }

        public override void InnerRestart()
        {
            timer = 0;
            for (int i = 0; i < children.Length; i++)
                children[i].Restart();
        }

        public override void Update()
        {
            if (children.Length != 2)
            {
                Debug.LogError("Children length of Guard node does not equal 2. This node will always return failure");
                CurrentStatus = Status.Failure;
                return;
            }

            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                if (children[0] is BoolNode)
                {
                    BoolNode boolNode = children[0] as BoolNode;
                    boolNode.Restart();
                    boolNode.Beginn(tree);
                    if (boolNode.IsFulfilled() == false)
                    {
                        CurrentStatus = Status.Failure;
                        return;
                    }
                    else
                    {
                        timer = checkEverySeconds;
                    }
                }
                else // children[0] is not a boolNode
                {
                    switch (children[0].CurrentStatus)
                    {
                        case Status.Running:
                            children[0].Update();
                            break;
                        case Status.Success:
                            timer = checkEverySeconds;
                            children[0].Restart();
                            children[0].Beginn(tree);
                            break;
                        case Status.Failure:
                            CurrentStatus = Status.Failure;
                            return;
                    }
                }
            }

            switch (children[1].CurrentStatus)
            {
                case Status.Running:
                    children[1].Update();
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
            GuardNode guardNode = CreateInstance<GuardNode>();
            guardNode.checkEverySeconds = checkEverySeconds;
            return guardNode;
        }
    }
}