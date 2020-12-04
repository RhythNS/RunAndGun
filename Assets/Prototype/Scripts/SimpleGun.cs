using UnityEngine;

[CreateAssetMenu(fileName = "New SimpleGun", menuName = "Weapons/Simple Gun")]
public class SimpleGun : Weapon
{
    [SerializeField] private float speed;
    [SerializeField] private int damage;
    [SerializeField] private float aliveTime;

    public override void Fire(GameObject shooter, Vector3 origin, Vector2 direction)
    {
        Instantiate(WeaponDict.Instance.bulletPrefab, origin, Quaternion.identity).Set(shooter, direction * speed, damage, aliveTime);
    }
}
