using UnityEngine;

[CreateAssetMenu(fileName = "New Sword", menuName = "Weapons/Sword")]
public class Sword : Weapon
{
    [SerializeField] private float aliveTime;
    [SerializeField] private float moveDistance;
    [SerializeField] private int damage;

    public override void Fire(GameObject shooter, Vector3 origin, Vector2 direction)
    {
        Quaternion rotation = Quaternion.identity;
        if (direction.x == 0 && direction.y > 0)
            rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        else if (direction.x == 0 && direction.y < 0)
            rotation = Quaternion.Euler(0.0f, 0.0f, -180.0f);
        else if (direction.x < 0 && direction.y == 0)
            rotation = Quaternion.Euler(0.0f, 0.0f, -270.0f);
        else if (direction.x > 0 && direction.y == 0)
            rotation = Quaternion.Euler(0.0f, 0.0f, -90.0f);

        Instantiate(WeaponDict.Instance.wavePrefab, new Vector3(0.0f, 0.0f, 0.0f), rotation).Set(shooter, aliveTime, direction.normalized * moveDistance, damage);
    }
}
