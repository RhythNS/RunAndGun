using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : Pickable
{
    public override PickableType PickableType => PickableType.Weapon;
}
