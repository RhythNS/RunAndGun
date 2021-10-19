using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetLookDirection : BNodeAdapter
{
    public enum Mode
    {
        UseHealth, UseDirection, UseAbsolutePosition
    }

    public override string StringToolTip => "Sets the firing direction of an player AI.";
    public override int MaxNumberOfChildren => 0;

    [SerializeField] private Mode mode;
    
    [SerializeField] private HealthValue healthTarget;
    private Health health;
    [SerializeField] private Vector2Value lookTarget;
    private Vector2 look;

    private PlayerMover playerMover;

    public override void InnerSetup()
    {
        playerMover = tree.AttachedBrain.GetComponent<PlayerMover>();
    }

    public override void InnerBeginn()
    {
        switch (mode)
        {
            case Mode.UseHealth:
                health = healthTarget.Get();
                break;
            case Mode.UseAbsolutePosition:
            case Mode.UseDirection:
                look = lookTarget.Get();
                break;
        }
    }

    public override void Update()
    {
        if (!playerMover)
        {
            CurrentStatus = Status.Failure;
            return;
        }

        switch (mode)
        {
            case Mode.UseHealth:
                if (!health)
                {
                    CurrentStatus = Status.Failure;
                    return;
                }
                playerMover.FireDirection = (health.transform.position - tree.AttachedBrain.transform.position);
                CurrentStatus = Status.Success;

                break;
            case Mode.UseDirection:
                playerMover.FireDirection = look;
                CurrentStatus = Status.Success;
                break;

            case Mode.UseAbsolutePosition:
                playerMover.FireDirection = (look - (Vector2)tree.AttachedBrain.transform.position);
                break;

            default:
                Debug.LogError("Missing implemented case: " + mode);
                CurrentStatus = Status.Failure;
                break;
        }
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        PlayerSetLookDirection psld = CreateInstance<PlayerSetLookDirection>();
        psld.healthTarget = (HealthValue)CloneValue(originalValueForClonedValue, healthTarget);
        psld.lookTarget = (Vector2Value)CloneValue(originalValueForClonedValue, lookTarget);
        psld.mode = mode;
        return psld;
    }

    protected override void InnerReplaceValues(Dictionary<Value, Value> originalReplace)
    {
        if (originalReplace.TryGetValue(lookTarget, out Value newLookTarget))
            lookTarget = (Vector2Value)newLookTarget;
        if (originalReplace.TryGetValue(healthTarget, out Value newHealthTarget))
            healthTarget = (HealthValue)newHealthTarget;
    }
}
