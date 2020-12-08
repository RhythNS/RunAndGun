using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceableWeapon : MonoBehaviour
{
    [SerializeField] public Weapon weapon;

    private void Start()
    {
        Instantiate(WeaponDict.Instance.GetWeaponInWorldForWeapon(weapon)).Drop(gameObject, weapon);
    }
}
