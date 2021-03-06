﻿using System.Collections.Generic;
using UnityEngine;

namespace Rhyth.BTree
{
    /// <summary>
    /// Runs the second node until the first node fails.
    /// </summary>
    public class GuardNode : BNodeAdapter
    {
        public override string StringToolTip => "Runs the second node until the first node fails.\nReturns success if the second node returns success.\nReturns failure if the first or second node failed.";

        public override string StringInEditor => "X";

        [SerializeField] private float checkEverySeconds = 1f;

        private float timer;
        private BoolNode convertedBoolNode;

        public override int MaxNumberOfChildren => 2;

        public override void InnerSetup()
        {
            if (children[0] is BoolNode)
                convertedBoolNode = children[0] as BoolNode;
        }

        public override void InnerBeginn()
        {
            children[0].Restart();
            children[0].Beginn();
        }

        public override void InnerRestart()
        {
            timer = 0;

            for (int i = 0; i < children.Length; i++)
                children[i].Restart();
        }

        public override void Update()
        {
            if (children.Length != 2)
            {
                Debug.LogError("Children length of Guard node does not equal 2. This node will always return failure");
                CurrentStatus = Status.Failure;
                return;
            }

            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                if (convertedBoolNode != null)
                {
                    convertedBoolNode.Restart();
                    convertedBoolNode.Beginn();
                    if (convertedBoolNode.IsFulfilled() == false)
                    {
                        CurrentStatus = Status.Failure;
                        return;
                    }
                    timer = checkEverySeconds;
                }
                else // children[0] is not a boolNode
                {
                    switch (children[0].CurrentStatus)
                    {
                        case Status.Running:
                            children[0].Update();
                            break;
                        case Status.Success:
                            timer = checkEverySeconds;
                            children[0].Restart();
                            children[0].Beginn();
                            break;
                        case Status.Failure:
                            CurrentStatus = Status.Failure;
                            return;
                    }
                }
            }

            switch (children[1].CurrentStatus)
            {
                case Status.Waiting:
                    children[1].Restart();
                    children[1].Beginn();
                    goto case Status.Running;
                case Status.Running:
                    children[1].Update();
                    break;
                case Status.Success:
                    CurrentStatus = Status.Success;
                    break;
                case Status.Failure:
                    CurrentStatus = Status.Failure;
                    break;
            }
        }

        protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
        {
            GuardNode guardNode = CreateInstance<GuardNode>();
            guardNode.checkEverySeconds = checkEverySeconds;
            return guardNode;
        }
    }
}