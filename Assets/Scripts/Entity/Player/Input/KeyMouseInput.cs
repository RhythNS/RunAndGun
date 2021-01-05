using System.Collections.Generic;
using UnityEngine;

public class KeyMouseInput : RAGInput
{
    public override InputType InputType => InputType.KeyMouse;

    protected override void OnStart()
    {
        Texture2D cursor = InputDict.Instance.MouseCursor;

        Cursor.visible = true;
        Cursor.SetCursor(cursor, new Vector2(cursor.width / 2, cursor.height / 2), CursorMode.Auto);
    }

    protected override bool HasFocusPoint => true;

    protected override Vector2 GetFocusPoint()
    {
        Vector2 mousePos = Input.mousePosition;
        mousePos.x = Mathf.Clamp(mousePos.x, 0.0f, Screen.width);
        mousePos.y = Mathf.Clamp(mousePos.y, 0.0f, Screen.height);
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    protected override Vector2 GetMovementInput()
    {
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    protected override bool GetDashInput()
    {
        return Input.GetKeyDown(KeyCode.LeftShift);
    }

    protected override bool GetFireInput(out Vector2 fireDirection)
    {
        if (Input.GetMouseButtonDown(0) == true)
        {
            fireDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            return true;
        }
        else
        {
            fireDirection = new Vector2(0.0f, 0.0f);
            return false;
        }
    }

    protected override void Pickup()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();
            List<Collider2D> colls = new List<Collider2D>();
            boxCollider2D.OverlapCollider(new ContactFilter2D().NoFilter(), colls);
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

    protected override bool GetReloadInput()
    {
        return Input.GetKeyDown(KeyCode.R);
    }

    public override void Remove()
    {
        Destroy(this);
    }

    private void OnDestroy()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

}
