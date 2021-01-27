using UnityEngine;

public abstract class WeaponAnimator : MonoBehaviour
{
    public abstract WeaponAnimatorType WeaponAnimatorType { get; }

    public Animator Animator { get; set; }

    protected WeaponPoints WeaponPoints { get; private set; }

    protected SpriteRenderer SpriteRenderer { get; private set; }

    private void Awake()
    {
        WeaponPoints = transform.parent.GetComponent<WeaponPoints>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();

        InnerAwake();
    }

    protected virtual void InnerAwake() { }

    public static WeaponAnimator Replace(WeaponAnimator oldAnimator, Weapon newWeapon)
    {
        GameObject gObj = oldAnimator.gameObject;
        WeaponAnimatorType weaponAnimatorType = newWeapon ? newWeapon.WeaponAnimatorType : WeaponAnimatorType.Null;
        
        Destroy(oldAnimator);
        WeaponAnimator newAnimator;

        switch (weaponAnimatorType)
        {
            case WeaponAnimatorType.SingleHand:
                newAnimator = gObj.AddComponent<SingleHandWeaponAnimator>();
                break;

            case WeaponAnimatorType.TwoHand:
                newAnimator = gObj.AddComponent<TwoHandWeaponAnimator>();
                break;

            case WeaponAnimatorType.Null:
                newAnimator = gObj.AddComponent<NullWeaponAnimator>();
                newAnimator.Animator.runtimeAnimatorController = null;
                return newAnimator;

            default:
                throw new System.Exception("WeaponAnimatorType " + weaponAnimatorType + " not implemented!");
        }

        newAnimator.Animator.runtimeAnimatorController = newWeapon.Animator;

        return newAnimator;
    }

    public virtual void OnSingleShotFired()
    {
        // Animator.SetTrigger("ShotSingleBullet");
    }

    public virtual void OnStartedFire()
    {
        // Animator.SetBool("Fire", true);
    }

    public virtual void OnStoppedFire()
    {
        // Animator.SetBool("Fire", false);
    }

    public virtual void OnStartedReload()
    {
        // Animator.SetBool("Reload", true);
    }

    public virtual void OnStoppedReload()
    {
        // Animator.SetBool("Reload", false);
    }

    public abstract void SetDirection(Vector2 direction);
}
