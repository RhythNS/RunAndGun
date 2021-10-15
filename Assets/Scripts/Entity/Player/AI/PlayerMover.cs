using UnityEngine;

public class PlayerMover : BrainMover
{
    public Vector2 CurrentMovementDirection { get; protected set; } = Vector2.zero;
    public bool DashRequest { get; set; } = false;

    public Vector2 FireDirection { get; set; } = Vector2.zero;
    public bool ShouldFire { get; set; } = false;
    public bool ReloadRequest { get; set; } = false;
    public bool ReviveRequest { get; set; } = false;

    protected override void Update()
    {
        switch (State)
        {
            case PathState.InProgress:
                {
                    Vector2 dir = transform.position;
                    dir = Destination - dir;
                    if (dir.sqrMagnitude < MAGNITUDE_SQUARED_TO_REACH)
                    {
                        State = PathState.Reached;
                    }

                    CurrentMovementDirection = dir.normalized;
                    break;
                }

            case PathState.ConstantDirection:
                {
                    CurrentMovementDirection = ConstantDirection;
                    break;
                }
            default:
                {
                    CurrentMovementDirection = Vector2.zero;
                    break;
                }
        }
    }
}
