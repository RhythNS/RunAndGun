using UnityEngine;

public class AIInput : RAGInput
{
    public override InputType InputType => InputType.AI;

    private PlayerMover playerMover;

    public override void Start()
    {
        base.Start();
        playerMover = GetComponent<PlayerMover>();
    }

    public override void Remove()
    {
        Destroy(this);
    }

    protected override bool GetDashInput()
    {
        if (playerMover.DashRequest)
        {
            playerMover.DashRequest = false;
            return true;
        }
        return false;
    }

    protected override bool GetFireInput(out Vector2 fireDirection)
    {
        fireDirection = playerMover.FireDirection;
        return playerMover.ShouldFire;
    }

    protected override Vector2 GetMovementInput()
    {
        return playerMover.ShouldMove ? playerMover.CurrentMovementDirection : Vector2.zero;
    }

    protected override bool GetReloadInput()
    {
        if (playerMover.ReloadRequest)
        {
            playerMover.ReloadRequest = false;
            return true;
        }
        return false;
    }

    protected override bool GetReviveInput()
    {
        if (playerMover.ReviveRequest)
        {
            playerMover.ReviveRequest = false;
            return true;
        }
        return false;
    }

    protected override void Pickup() { }
}
