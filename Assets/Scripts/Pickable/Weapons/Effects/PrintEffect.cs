using UnityEngine;

[CreateAssetMenu(menuName = "Pickable/Weapon/Effect/PrintEffect")]
public class PrintEffect : Effect
{
    public override void OnHit(Weapon weapon, Health affecter, Health health)
    {
        Debug.Log(affecter.gameObject.name + " with weapon " + weapon.name + " hit " + health.gameObject.name +
            " with a damage of " + weapon.BaseDamage);
    }
}
