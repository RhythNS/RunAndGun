using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rhyth.BTree
{
    public class AndNode : BoolNode
    {
        [SerializeField] private bool inverseResult = false;

        public override int MaxNumberOfChildren => -1;

        private static readonly Type[] allowedChildrenTypes = { typeof(BoolNode) };
        public override Type[] AllowedChildrenTypes => allowedChildrenTypes;

        public override void InnerBeginn()
        {
            for (int i = 0; i < children.Length; i++)
                children[i].Beginn(tree);
        }

        public override void InnerRestart()
        {
            for (int i = 0; i < children.Length; i++)
                children[i].Restart();
        }

        protected override bool InnerIsFulfilled()
        {
            for (int i = 0; i < children.Length; i++)
            {
                if (((BoolNode)children[i]).IsFulfilled() == false)
                    return inverseResult;
            }
            return !inverseResult;
        }

        protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
        {
            AndNode and = CreateInstance<AndNode>();
            and.inverseResult = inverseResult;
            return and;
        }
    }
}