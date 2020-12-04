using UnityEngine;

[CreateAssetMenu(fileName = "New AOEGun", menuName = "Weapons/AOEGun")]
public class AOEGun : Weapon
{
    [SerializeField] private float speed;
    [SerializeField] private int layers;
    [SerializeField] private float aliveTime;
    [SerializeField] private int damage;
    [SerializeField] float velocityAngleChange;
    [SerializeField] int bulletsFromStray;

    public override void Fire(GameObject shooter, Vector3 origin, Vector2 direction)
    {
        Instantiate(WeaponDict.Instance.aoeBullet, origin, Quaternion.identity).Set(shooter.GetComponent<Collider2D>(), true, velocityAngleChange, bulletsFromStray, speed, layers, aliveTime, direction, damage);
    }
}
