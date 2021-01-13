using System.Collections.Generic;
using UnityEngine;

namespace Rhyth.BTree
{
    public class RepeatNode : BNodeAdapter
    {
        public override string StringInEditor => "↻";

        public override int MaxNumberOfChildren => 1;

        [SerializeField] private int toRepeat;
        [SerializeField] private bool failOnNodeFailure = true;

        private int at;

        public override void InnerBeginn()
        {
            at = 0;
            if (children.Length == 1)
                children[0].Beginn(tree);
        }

        public override void InnerRestart()
        {
            at = 0;
            if (children.Length == 1)
                children[0].Restart();
        }

        public override void Update()
        {
            if (children.Length == 0)
            {
                CurrentStatus = Status.Success;
                return;
            }
            switch (children[0].CurrentStatus)
            {
                case Status.Running:
                    children[0].Update();
                    break;
                case Status.Failure:
                    if (failOnNodeFailure)
                        CurrentStatus = Status.Failure;
                    else
                        ResetNode();
                    break;
                case Status.Success:
                    ResetNode();
                    break;
            }
        }

        private void ResetNode()
        {
            if (++at == toRepeat)
            {
                CurrentStatus = Status.Success;
            }
            else
            {
                children[0].Restart();
                children[0].Beginn(tree);
            }
        }

        protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
        {
            RepeatNode repeat = CreateInstance<RepeatNode>();
            repeat.toRepeat = toRepeat;
            repeat.failOnNodeFailure = failOnNodeFailure;
            return repeat;
        }
    }
}