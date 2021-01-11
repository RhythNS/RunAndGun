using Rhyth.BTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GotoNode : BNodeAdapter
{
    public override int MaxNumberOfChildren => 0;

    [SerializeField] private Vector2 destPos;
    [SerializeField] private float requiredTime;

    private Vector2 startPos;
    private float timer;

    public override void InnerBeginn()
    {
        timer = 0;
        startPos = tree.AttachedBrain.transform.position;
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        float perc = timer / requiredTime;

        Vector3 pos = tree.AttachedBrain.transform.position;
        Vector2 lerpPos = Vector2.Lerp(startPos, destPos, perc);
        pos.x = lerpPos.x;
        pos.y = lerpPos.y;
        tree.AttachedBrain.transform.position = pos;

        if (perc >= 1.0f)
            CurrentStatus = Status.Success;
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        GotoNode cloned = CreateInstance<GotoNode>();
        cloned.destPos = destPos;
        cloned.requiredTime = requiredTime;
        return cloned;
    }
}
