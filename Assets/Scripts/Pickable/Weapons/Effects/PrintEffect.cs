using UnityEngine;

[CreateAssetMenu(menuName = "Pickable/Weapon/Effect/PrintEffect")]
public class PrintEffect : Effect
{
    public override void OnHit(Weapon weapon, Health health)
    {
        Debug.Log(weapon.name + " hit " + health.gameObject.name + " with a damage of " + weapon.BaseDamage);
    }
}
