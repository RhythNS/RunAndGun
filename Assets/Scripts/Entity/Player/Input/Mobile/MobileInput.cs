using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInput : RAGInput
{
    public override InputType InputType => InputType.Mobile;

    private MobileUIManager mobileUI;

    protected override void OnStart()
    {
        GameObject canvasObj = GameObject.Find("Canvas"); // TOOD: CHANGE THIS ONCE UI IS IMPLEMENTED!
        mobileUI = Instantiate(InputDict.Instance.MobileUIManagerPrefab, canvasObj.transform);
    }

    protected override bool GetDashInput()
    {
        // TODO: Implement
        return false;
    }

    protected override bool GetFireInput(out Vector2 fireDirection)
    {
        fireDirection = mobileUI.Aim.Output;
        return mobileUI.Aim.Down;
    }

    protected override Vector2 GetMovementInput()
    {
        return mobileUI.Move.Output;
    }

    protected override bool GetReloadInput()
    {
        // TODO: Implement
        return Input.GetKeyDown(KeyCode.R);
    }

    protected override void Pickup()
    {
        // TODO: Implement
    }

    public override void Remove()
    {
        Destroy(mobileUI.gameObject);
    }

}
