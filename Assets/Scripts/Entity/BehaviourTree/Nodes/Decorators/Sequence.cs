using System.Collections.Generic;

namespace Rhyth.BTree
{
    public class Sequence : BNodeAdapter
    {
        public override string StringToolTip => "Runs its children one by one.\nReturns success if all children returned success. Returns failure if one child return failure.";

        public override int MaxNumberOfChildren => -1;
        public override string StringInEditor => "->";

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
                    if (++at == children.Length)
                        CurrentStatus = Status.Success;
                    else
                        children[at].Beginn(tree);
                    break;
                case Status.Failure:
                    CurrentStatus = Status.Failure;
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
            => CreateInstance<Sequence>();
    }
}