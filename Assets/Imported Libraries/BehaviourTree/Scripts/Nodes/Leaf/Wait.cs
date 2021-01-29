using System.Collections.Generic;
using UnityEngine;

namespace Rhyth.BTree
{
    public class Wait : BNodeAdapter
    {
        public override string StringToolTip => "Waits a given amount of seconds.\nReturns success if the value successOnFinish is true.";
        public override int MaxNumberOfChildren => 0;

        [SerializeField] private float timeToWait = 1;
        [SerializeField] private bool successOnFinish = true;

        private float timer;

        public override void InnerRestart()
        {
            timer = timeToWait;
        }

        public override void Update()
        {
            timer -= Time.deltaTime;
            if (timer < 0)
                CurrentStatus = successOnFinish ? Status.Success : Status.Failure;
        }

        protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
        {
            Wait wait = CreateInstance<Wait>();
            wait.timeToWait = timeToWait;
            wait.successOnFinish = successOnFinish;
            return wait;
        }
    }
}
