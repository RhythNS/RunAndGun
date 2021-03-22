using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pickable/Weapon/BulletSpawn/Spray")]
public class SprayBulletSpawn : BulletSpawnModel
{
    [SerializeField][Tooltip("The amount of pellets in a single shot.")]
    private uint pelletAmount;

    [SerializeField][Tooltip("In degrees in front of the weapon.")]
    private float sprayArc;

    protected override void InnerShoot(EquippedWeapon weapon)
    {
        float currPelletArc = sprayArc * -0.5f;
        for (int i = 0; i < pelletAmount; i++) {
            Vector2 vec = Quaternion.AngleAxis(currPelletArc + GetAccuracyAngle(weapon.Weapon.Accuracy), Vector3.forward) * weapon.LocalDirection;
            weapon.SpawnNew(vec);

            currPelletArc += sprayArc / pelletAmount;
        }
    }
}
