using UnityEngine;

public class MobileInput : RAGInput
{
    public override InputType InputType => InputType.Mobile;

    private MobileUIManager mobileUI;

    public void SetUI(MobileUIManager ui) => mobileUI = ui;

    protected override void OnStart()
    {
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

    protected override bool GetReviveInput()
    {
        // TODO: Implement
        return false;
    }
}
