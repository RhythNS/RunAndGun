using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : Pickable
{
    public override PickableType PickableType => PickableType.Item;
}
