using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTest : NetworkBehaviour
{
    public override void OnStartServer()
    {
        Pickable weapon = PickableDict.Instance.Get(PickableType.Weapon, 1);
        PickableInWorld.Place(weapon, new Vector3(0.0f, 0.0f, 0.0f));
    }
}
