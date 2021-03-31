using FMODUnity;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Pickable/Weapon/Weapon")]
public class Weapon : Pickable
{
    public override PickableType PickableType => PickableType.Weapon;

    public Effect[] Effects => effects;
    [SerializeField] private Effect[] effects;

    public ShotModel ShotModel => shotModel;
    [SerializeField] private ShotModel shotModel;

    public BulletInfo BulletInfo => bulletInfo;
    [SerializeField] private BulletInfo bulletInfo;

    public BulletPath BulletPath => bulletPath;
    [SerializeField] private BulletPath bulletPath;

    public BulletSpawnModel BulletSpawnModel => bulletSpawnModel;
    [SerializeField] private BulletSpawnModel bulletSpawnModel;

    public RuntimeAnimatorController Animator => animator;
    [SerializeField] private RuntimeAnimatorController animator;

    public WeaponAnimatorType WeaponAnimatorType => weaponAnimatorType;
    [SerializeField] private WeaponAnimatorType weaponAnimatorType;

    public TargetMode TargetMode => targetMode;
    [SerializeField] private TargetMode targetMode;

    public bool UseLocalSpace => useLocalSpace;
    [SerializeField] private bool useLocalSpace;

    public float Range => range;
    [SerializeField] private float range;

    public float Accuracy => accuracy;
    [SerializeField]
    [Range(0f, 100f)]
    [Tooltip("The accuracy for each spawned bullet. 100 = fully accurate, 0 = differs up to 45° from original trajectory")]
    private float accuracy;

    public float Speed => speed;
    [SerializeField] private float speed;

    public int BaseDamage => baseDamage;
    [SerializeField] private int baseDamage;

    public int MagazineSize => magazineSize;
    [SerializeField] private int magazineSize;

    public float ReloadTime => reloadTime;
    [SerializeField] private float reloadTime;

    public WeaponSoundModel WeaponSoundModel => weaponSoundModel;
    [SerializeField] private WeaponSoundModel weaponSoundModel;

    public IEnumerator Shoot(EquippedWeapon equippedWeapon)
    {
        return shotModel.Shoot(equippedWeapon);
    }
}
