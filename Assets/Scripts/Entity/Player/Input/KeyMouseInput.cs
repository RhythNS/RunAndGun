using System.Collections;
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

    public override void Remove()
    {
        Destroy(this);
    }

    private void OnDestroy()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
