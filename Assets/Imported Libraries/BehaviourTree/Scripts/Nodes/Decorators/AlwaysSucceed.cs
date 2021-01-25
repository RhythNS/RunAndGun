using System.Collections.Generic;

namespace Rhyth.BTree
{
    public class AlwaysSuccesed : BNodeAdapter
    {
        public override string StringToolTip => "Runs a node and always returns success.";
        public override int MaxNumberOfChildren => 1;

        public override void InnerRestart()
        {
            if (children.Length == 1)
                children[0].Restart();
        }

        public override void InnerBeginn()
        {
            if (children.Length == 1)
                children[0].Beginn(tree);
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
                case Status.Success:
                    CurrentStatus = Status.Success;
                    break;
                case Status.Failure:
                    CurrentStatus = Status.Success;
                    break;
            }
        }

        protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
            => CreateInstance<AlwaysSuccesed>();
    }
}