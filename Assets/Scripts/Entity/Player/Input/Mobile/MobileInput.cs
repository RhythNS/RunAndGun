using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInput : RAGInput
{
    public override InputType InputType => InputType.Mobile;

    public override void Remove()
    {
        throw new System.NotImplementedException();
    }

    protected override bool GetDashInput()
    {
        throw new System.NotImplementedException();
    }

    protected override bool GetFireInput(out Vector2 fireDirection)
    {
        throw new System.NotImplementedException();
    }

    protected override Vector2 GetMovementInput()
    {
        throw new System.NotImplementedException();
    }

    protected override bool GetReloadInput()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnStart()
    {
        throw new System.NotImplementedException();
    }

    protected override void Pickup()
    {
        throw new System.NotImplementedException();
    }
}
