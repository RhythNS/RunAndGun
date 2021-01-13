using System.Collections.Generic;
using UnityEngine;

namespace Rhyth.BTree
{
    public class WaitRandom : BNodeAdapter
    {
        public override int MaxNumberOfChildren => 0;

        [SerializeField] private Vector2 timeToWaitRange;
        [SerializeField] private bool successOnFinish = true;

        private float timer;

        public override void InnerRestart()
        {
            timer = Random.Range(timeToWaitRange.x, timeToWaitRange.y);
        }

        public override void Update()
        {
            timer -= Time.deltaTime;
            if (timer < 0)
                CurrentStatus = successOnFinish ? Status.Success : Status.Failure;
        }

        protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
        {
            WaitRandom wait = CreateInstance<WaitRandom>();
            wait.timeToWaitRange = timeToWaitRange;
            wait.successOnFinish = successOnFinish;
            return wait;
        }
    }
}