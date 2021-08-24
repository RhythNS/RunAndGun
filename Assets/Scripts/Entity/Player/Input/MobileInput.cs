using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Input used for mobile devices.
/// </summary>
public class MobileInput : RAGInput
{
    public override InputType InputType => InputType.Mobile;

    private MobileUIManager mobileUI;

    private bool pickUp = false;
    private bool revive = false;
    private bool dash = false;

    /// <summary>
    /// Sets reference to the mobile ui manager.
    /// </summary>
    public void SetUI(MobileUIManager ui)
    {
        mobileUI = ui;
        ui.DashButton.onClick.AddListener(OnDashButton);
        ui.ReviveButton.onClick.AddListener(OnReviveButton);
        ui.PickUpButton.onClick.AddListener(OnPickUpButton);
    }

    public void OnPickUpButton()
    {
        pickUp = true;
    }

    public void OnReviveButton()
    {
        revive = true;
    }

    public void OnDashButton()
    {
        dash = true;
    }

    protected override void OnStart()
    {
    }

    protected override bool GetDashInput()
    {
        if (dash)
        {
            dash = false;
            return true;
        }
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
        return Input.GetKeyDown(KeyCode.R);
    }

    protected override void Pickup()
    {
        if (pickUp)
        {
            pickUp = false;

            Collider2D collider2D = GetComponent<Collider2D>();
            List<Collider2D> colls = new List<Collider2D>();
            collider2D.OverlapCollider(new ContactFilter2D().NoFilter(), colls);
            for (int i = 0; i < colls.Count; i++)
            {
                if (colls[i].gameObject.TryGetComponent(out PickableInWorld _))
                {
                    Player.CmdPickup(colls[i].gameObject);
                    return;
                }
            }
        }
    }

    public override void Remove()
    {
        mobileUI.DashButton.onClick.RemoveListener(OnDashButton);
        mobileUI.ReviveButton.onClick.RemoveListener(OnReviveButton);
        mobileUI.PickUpButton.onClick.RemoveListener(OnPickUpButton);
        Destroy(mobileUI.gameObject);
    }

    protected override bool GetReviveInput()
    {
        if (revive)
        {
            revive = false;
            return true;
        }
        return false;
    }
}
