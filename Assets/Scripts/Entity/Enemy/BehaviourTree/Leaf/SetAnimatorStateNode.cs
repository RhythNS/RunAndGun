using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets an animator value.
/// </summary>
public class SetAnimatorStateNode : BNodeAdapter
{
    public override int MaxNumberOfChildren => 0;

    [SerializeField] private bool useTrigger;

    [SerializeField] private string animatorPropertyName;
    [SerializeField] private bool boolValue;

    private Animator animator;

    public override string StringToolTip => "Sets an animator value.";

    public override void InnerSetup()
    {
        animator = Brain.GetComponent<Animator>();
        if (!animator)
            Debug.LogError("Animator not found!");
    }

    public override void Update()
    {
        if (!animator)
            return;
        if (useTrigger)
            animator.SetTrigger(animatorPropertyName);
        else
            animator.SetBool(animatorPropertyName, boolValue);
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        SetAnimatorStateNode sasn = CreateInstance<SetAnimatorStateNode>();
        sasn.useTrigger = useTrigger;
        sasn.animatorPropertyName = animatorPropertyName;
        sasn.boolValue = boolValue;
        return sasn;
    }
}
