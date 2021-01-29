using System;
using System.Collections.Generic;

namespace Rhyth.BTree
{
    /// <summary>
    /// Inverts the status of a node
    /// </summary>
    public class BoolInverter : BoolNode
    {
        public override string StringToolTip => "Child must be a BoolNode!\nReturns success if the child failed. Returns failure if the child succssedes.";
        public override int MaxNumberOfChildren => 1;

        private static readonly Type[] allowedChildrenTypes = { typeof(BoolNode) };
        public override Type[] AllowedChildrenTypes => allowedChildrenTypes;

        public override void InnerBeginn()
        {
            if (children.Length == 1)
                children[0].Beginn();
        }

        public override void InnerRestart()
        {
            if (children.Length == 1)
                children[0].Restart();
        }

        protected override bool InnerIsFulfilled()
        {
            if (children.Length == 0)
                return true;
            else
                return !((BoolNode)children[0]).IsFulfilled();
        }

        protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
            => CreateInstance<BoolInverter>();
    }
}
