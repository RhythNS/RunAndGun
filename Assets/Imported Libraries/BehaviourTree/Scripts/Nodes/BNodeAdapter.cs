using System.Collections.Generic;

namespace Rhyth.BTree
{
    /// <summary>
    /// Utility class to inherit from if a Node does not use every method.
    /// </summary>
    public abstract class BNodeAdapter : BNode
    {
        public override void InnerSetup() { }

        public override void InnerRestart() { }

        public override void InnerBeginn() { }

        protected override void InnerReplaceValues(Dictionary<Value, Value> originalReplace) { }
    }
}