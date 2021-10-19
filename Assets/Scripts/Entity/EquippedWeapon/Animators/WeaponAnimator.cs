using UnityEngine;

/// <summary>
/// Animates the weapon hold in the entitys hand.
/// </summary>
public abstract class WeaponAnimator : MonoBehaviour
{
    public abstract WeaponAnimatorType WeaponAnimatorType { get; }

    public Animator Animator { get; set; }
    protected WeaponPoints WeaponPoints { get; private set; }
    protected SpriteRenderer SpriteRenderer { get; private set; }
    protected EntityAnimationController EntityAnimationController { get; private set; }
    protected Direction EntityDirection => EntityAnimationController ? EntityAnimationController.CurDirection : Direction.Down;

    private void Awake()
    {
        WeaponPoints = transform.parent.GetComponent<WeaponPoints>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        SpriteRenderer.flipX = false;
        SpriteRenderer.flipY = false;

        EntityAnimationController = transform.parent.GetComponent<EntityAnimationController>();
        if (EntityAnimationController)
            EntityAnimationController.DirectionChanged += EntityDirectionChanged;

        Animator = GetComponent<Animator>();

        InnerAwake();
    }

    private void OnDestroy()
    {
        if (EntityAnimationController)
            EntityAnimationController.DirectionChanged -= EntityDirectionChanged;
    }

    protected virtual void InnerAwake() { }

    /// <summary>
    /// Replaces the old Weapon animator with a new one.
    /// </summary>
    /// <param name="oldAnimator">The old animator.</param>
    /// <param name="newWeapon">The new weapon.</param>
    /// <returns>A reference to the new weapon animator.</returns>
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

    /// <summary>
    /// Called when the weapon fired.
    /// </summary>
    public virtual void OnSingleShotFired()
    {
        // Animator.SetTrigger("ShotSingleBullet");
    }

    /// <summary>
    /// Called when the weapon has started firing.
    /// </summary>
    public virtual void OnStartedFire()
    {
        // Animator.SetBool("Fire", true);
    }

    /// <summary>
    /// Called when the weapon has stopped firing.
    /// </summary>
    public virtual void OnStoppedFire()
    {
        // Animator.SetBool("Fire", false);
    }

    /// <summary>
    /// Called when the weapon has started reloading.
    /// </summary>
    public virtual void OnStartedReload()
    {
        // Animator.SetBool("Reload", true);
    }

    /// <summary>
    /// Called when the weapon has stopped reloading.
    /// </summary>
    public virtual void OnStoppedReload()
    {
        // Animator.SetBool("Reload", false);
    }

    /// <summary>
    /// Sets the direction of where the entity is aiming at.
    /// </summary>
    public abstract void SetDirection(Vector2 direction);

    public void EntityDirectionChanged(Direction direction)
    {
        Vector3 pos = transform.localPosition;
        switch (direction)
        {
            case Direction.Up:
            case Direction.Right:
                pos.z = 0.01f;
                break;

            case Direction.Down:
            case Direction.Left:
                pos.z = -0.01f;
                break;
        }

        transform.localPosition = pos;
    }
}
