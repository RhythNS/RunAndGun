using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pickable/Weapon/ShotModel/BurstFire")]
public class BurstFireModel : ShotModel
{
    protected override IEnumerator InnerShoot(EquippedWeapon equipped)
    {
        throw new System.NotImplementedException();
    }
}
