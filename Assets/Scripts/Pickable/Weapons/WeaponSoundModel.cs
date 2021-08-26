using FMODUnity;
using UnityEngine;

/// <summary>
/// Holds all sounds that a weapon can make.
/// </summary>
[CreateAssetMenu(menuName = "Pickable/Weapon/Sound Model")]
public class WeaponSoundModel : ScriptableObject
{
    /// <summary>
    /// Sounds when it reloads.
    /// </summary>
    public string ReloadSound => reloadSound;
    [SerializeField] [EventRef] private string reloadSound;

    /// <summary>
    /// Sound when a shot is attempted to be fired even though the weapon
    /// does not have any bullets left in the magazine.
    /// </summary>
    public string EmptyClipSound => emptyClipSound;
    [SerializeField] [EventRef] private string emptyClipSound;

    /// <summary>
    /// Sound when the weapon is fired.
    /// </summary>
    public string ShootSound => shootSound;
    [SerializeField] [EventRef] private string shootSound;

    /// <summary>
    /// Sound when the weapon is equipped.
    /// </summary>
    public string EquipSound => equipSound;
    [SerializeField] [EventRef] private string equipSound;

    /// <summary>
    /// Sound when the bullet of the weapon impacted something.
    /// </summary>
    public string ImpactSound => impactSound;
    [SerializeField] [EventRef] private string impactSound;

    /// <summary>
    /// Sound of the bullet when it is flying in the air.
    /// </summary>
    public string InAirLoop => inAirLoop;
    [SerializeField] [EventRef] private string inAirLoop;
}
