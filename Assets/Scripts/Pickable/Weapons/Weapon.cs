using Mirror;
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

    public BulletSpawnModel BulletSpawnModel => bulletSpawnModel;
    [SerializeField] private BulletSpawnModel bulletSpawnModel;

    public Bullet Bullet => bullet;
    [SerializeField] private Bullet bullet;

    public RuntimeAnimatorController Animator => animator;
    [SerializeField] private RuntimeAnimatorController animator;

    public bool UseLocalSpace => useLocalSpace;
    [SerializeField] private bool useLocalSpace;

    public float Range => range;
    [SerializeField] private float range;

    public float Speed => speed;
    [SerializeField] private float speed;

    public int BaseDamage => baseDamage;
    [SerializeField] private int baseDamage;

    public int MagazineSize => magazineSize;
    [SerializeField] private int magazineSize;

    [Server]
    public IEnumerator Shoot(Health shooter, EquippedWeapon equippedWeapon, Vector3 position, Vector2 direction)
    {
        return shotModel.Shoot(shooter, equippedWeapon, position, direction);
    }
}
