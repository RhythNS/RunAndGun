namespace Rhyth.BTree
{
    public abstract class BoolNode : BNodeAdapter
    {
        public override int MaxNumberOfChildren => 0;

        public sealed override void Update() => IsFulfilled();

        public bool IsFulfilled()
        {
            bool fulfilled = InnerIsFulfilled();
            CurrentStatus = fulfilled ? Status.Success : Status.Failure;
            return fulfilled;
        }

        protected abstract bool InnerIsFulfilled();
    }
}