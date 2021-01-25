using System.Collections.Generic;
using UnityEngine;

namespace Rhyth.BTree
{
    public class ForeverNode : BNodeAdapter
    {
        public override string StringToolTip => "Executes a node forever.\nThere is no return value.";

        public override string StringInEditor => "∞";

        public override int MaxNumberOfChildren => 1;

        public override void InnerBeginn()
        {
            if (children.Length == 1)
                children[0].Beginn(tree);
        }

        public override void InnerRestart()
        {
            if (children.Length == 1)
                children[0].Restart();
        }

        public override void Update()
        {
            if (children.Length == 0)
            {
                Debug.LogWarning(tree.AttachedBrain.name +
                    "'s Tree has a ForeverNode with not children. It will always return failure");
                CurrentStatus = Status.Failure;
            }
            switch (children[0].CurrentStatus)
            {
                case Status.Running:
                    children[0].Update();
                    break;
                case Status.Success:
                case Status.Failure:
                    children[0].Restart();
                    children[0].Beginn(tree);
                    break;
            }
        }

        protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
            => CreateInstance<ForeverNode>();
    }
}