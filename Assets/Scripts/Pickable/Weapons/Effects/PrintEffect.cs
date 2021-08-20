using UnityEngine;

[CreateAssetMenu(menuName = "Pickable/Weapon/Effect/PrintEffect")]
public class PrintEffect : Effect
{
    public override void OnHit(Weapon weapon, Bullet bullet, Health health)
    {
        Debug.Log(bullet.ShooterHealth.gameObject.name + " with weapon " + weapon.name + " hit " + health.gameObject.name +
            " with a damage of " + weapon.BaseDamage);
    }
}
