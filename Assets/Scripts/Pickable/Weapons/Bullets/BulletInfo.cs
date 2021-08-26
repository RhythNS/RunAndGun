using UnityEngine;

/// <summary>
/// Info about bullets.
/// </summary>
[CreateAssetMenu(menuName = "Pickable/Weapon/Bullet Info")]
public class BulletInfo : ScriptableObject
{
    /// <summary>
    /// What the bullet looks like.
    /// </summary>
    public Sprite Sprite => sprite;
    [SerializeField] private Sprite sprite;

    /// <summary>
    /// The impact effects that occur when a bullet hit something.
    /// </summary>
    public BulletImpactEffect[] BulletImpactEffects => bulletImpactEffects;
    [SerializeField] private BulletImpactEffect[] bulletImpactEffects;
}
