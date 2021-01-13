using System.Collections.Generic;
using UnityEngine;

namespace Rhyth.BTree
{
    public class PrintNode : BNodeAdapter
    {
        public override int MaxNumberOfChildren => 0;

        [SerializeField] private string toPrint;

        public override void Update()
        {
            Debug.Log(toPrint);
            CurrentStatus = Status.Success;
        }

        protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
        {
            PrintNode printNode = CreateInstance<PrintNode>();
            printNode.toPrint = toPrint;
            return printNode;
        }
    }
}
