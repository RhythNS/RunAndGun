using System.Collections;
using UnityEngine;

/// <summary>
/// A weapon that can spawn bullets to affect other entities.
/// </summary>
[CreateAssetMenu(menuName = "Pickable/Weapon/Weapon")]
public class Weapon : Pickable
{
    public override PickableType PickableType => PickableType.Weapon;

    /// <summary>
    /// All effects that occur when the bullet hit the targeted entity.
    /// </summary>
    public Effect[] Effects => effects;
    [SerializeField] private Effect[] effects;

    /// <summary>
    /// Describes how the weapon spawns bullets.
    /// </summary>
    public ShotModel ShotModel => shotModel;
    [SerializeField] private ShotModel shotModel;

    /// <summary>
    /// Information about the bullet.
    /// </summary>
    public BulletInfo BulletInfo => bulletInfo;
    [SerializeField] private BulletInfo bulletInfo;

    /// <summary>
    /// How the bullet flies through the air.
    /// </summary>
    public BulletPath BulletPath => bulletPath;
    [SerializeField] private BulletPath bulletPath;

    /// <summary>
    /// How bullets spawn when spawned by the ShotModel.
    /// </summary>
    public BulletSpawnModel BulletSpawnModel => bulletSpawnModel;
    [SerializeField] private BulletSpawnModel bulletSpawnModel;

    /// <summary>
    /// The animator for animating the weapon held in hands.
    /// </summary>
    public RuntimeAnimatorController Animator => animator;
    [SerializeField] private RuntimeAnimatorController animator;

    /// <summary>
    /// What type of animator the equipped weapon should use.
    /// </summary>
    public WeaponAnimatorType WeaponAnimatorType => weaponAnimatorType;
    [SerializeField] private WeaponAnimatorType weaponAnimatorType;

    /// <summary>
    /// What the bullets of the weapon hits.
    /// </summary>
    public TargetMode TargetMode => targetMode;
    [SerializeField] private TargetMode targetMode;

    /// <summary>
    /// Wheter bullets fly in local space.
    /// </summary>
    public bool UseLocalSpace => useLocalSpace;
    [SerializeField] private bool useLocalSpace;

    /// <summary>
    /// The maximum range before bullets are deleted.
    /// </summary>
    public float Range => range;
    [SerializeField] private float range;

    /// <summary>
    /// The accuracy for each spawned bullet. 100 = fully accurate, 0 = differs up to 45° from original trajectory
    /// </summary>
    public float Accuracy => accuracy;
    [SerializeField]
    [Range(0f, 100f)]
    [Tooltip("The accuracy for each spawned bullet. 100 = fully accurate, 0 = differs up to 45° from original trajectory")]
    private float accuracy;

    /// <summary>
    /// The speed in units per second of a bullet.
    /// </summary>
    public float Speed => speed;
    [SerializeField] private float speed;

    /// <summary>
    /// The base damage the weapon does.
    /// </summary>
    public int BaseDamage => baseDamage;
    [SerializeField] private int baseDamage;

    /// <summary>
    /// How many bullets a weapon can fire before needing to be reloaded.
    /// </summary>
    public int MagazineSize => magazineSize;
    [SerializeField] private int magazineSize;

    /// <summary>
    /// How long the weapon must reload for in seconds.
    /// </summary>
    public float ReloadTime => reloadTime;
    [SerializeField] private float reloadTime;

    /// <summary>
    /// Sounds that weapon makes.
    /// </summary>
    public WeaponSoundModel WeaponSoundModel => weaponSoundModel;
    [SerializeField] private WeaponSoundModel weaponSoundModel;

    /// <summary>
    /// Shoots the weapon.
    /// </summary>
    /// <param name="equippedWeapon">A reference to the equipped weapon that fired.</param>
    public IEnumerator Shoot(EquippedWeapon equippedWeapon)
    {
        return shotModel.Shoot(equippedWeapon);
    }
}
