using System.Collections.Generic;

namespace Rhyth.BTree
{
    public class Selector : BNodeAdapter
    {
        public override string StringToolTip => "Runs its children one by one.\nReturns success if one child returned success. Returns failure if every child return failure.";

        public override int MaxNumberOfChildren => -1;
        public override string StringInEditor => "?";

        private int at;

        public override void InnerBeginn()
        {
            at = 0;
            children[at].Beginn(tree);
        }

        public override void Update()
        {
            switch (children[at].CurrentStatus)
            {
                case Status.Running:
                    children[at].Update();
                    break;
                case Status.Success:
                    CurrentStatus = Status.Success;
                    break;
                case Status.Failure:
                    if (++at == children.Length)
                        CurrentStatus = Status.Failure;
                    else
                        children[at].Beginn(tree);
                    break;
            }
        }

        public override void InnerRestart()
        {
            for (int i = 0; i < children.Length; i++)
                children[i].Restart();
            at = 0;
        }

        protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
            => CreateInstance<Selector>();
    }
}