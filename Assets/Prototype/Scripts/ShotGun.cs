using UnityEngine;

[CreateAssetMenu(fileName = "New ShotGun", menuName = "Weapons/Shot Gun")]
public class ShotGun : Weapon
{
    [SerializeField] private float speed;
    [SerializeField] private int damage;
    [SerializeField] private float aliveTime;
    [SerializeField] private int amount;
    [SerializeField] private float angleInc;

    public override void Fire(GameObject shooter, Vector3 origin, Vector2 direction)
    {
        for (float i = angleInc * -amount / 2; i < angleInc * amount / 2; i += angleInc)
        {
            Vector2 dir = Rotate(direction, i);
            Instantiate(WeaponDict.Instance.bulletPrefab, origin, Quaternion.identity).Set(shooter, dir * speed, damage, aliveTime);
        }
    }


    public static Vector2 Rotate(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

}
