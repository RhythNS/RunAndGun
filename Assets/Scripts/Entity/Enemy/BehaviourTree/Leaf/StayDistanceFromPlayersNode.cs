using Rhyth.BTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayDistanceFromPlayersNode : BNodeAdapter
{
    public override int MaxNumberOfChildren => 0;

    public override void InnerBeginn()
    {
        throw new System.NotImplementedException();
    }

    public override void InnerRestart()
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        throw new System.NotImplementedException();
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        throw new System.NotImplementedException();
    }

    protected override void InnerReplaceValues(Dictionary<Value, Value> originalReplace)
    {
        throw new System.NotImplementedException();
    }
}
