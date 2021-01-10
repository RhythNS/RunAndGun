using System.Collections.Generic;

namespace Rhyth.BTree
{
    public class While : BNodeAdapter
    {
        public override int MaxNumberOfChildren => 2;

        public override void InnerBeginn()
        {
            if (children.Length == 2)
            {
                children[0].Beginn(tree);
                children[1].Beginn(tree);
            }
        }

        public override void InnerRestart()
        {
            if (children.Length == 2)
            {
                children[0].Restart();
                children[1].Restart();
            }
        }

        public override void Update()
        {
            if (children.Length != 2)
            {
                CurrentStatus = Status.Failure;
                return;
            }

            for (int i = 0; i < children.Length; i++)
            {
                switch (children[i].CurrentStatus)
                {
                    case Status.Running:
                        children[i].Update();
                        break;
                    case Status.Success:
                        children[i].Restart();
                        children[i].Beginn(tree);
                        break;
                    case Status.Failure:
                        CurrentStatus = Status.Failure;
                        return;
                }
            }
        }

        protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
            => CreateInstance<While>();
    }
}
