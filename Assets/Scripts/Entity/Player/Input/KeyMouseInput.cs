using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyMouseInput : Input
{
    public override InputType InputType => InputType.KeyMouse;

    private void Awake()
    {

    }

    public override void Remove()
    {
        Destroy(this);
    }

}
