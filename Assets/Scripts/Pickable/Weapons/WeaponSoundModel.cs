using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "Pickable/Weapon/Sound Model")]
public class WeaponSoundModel : ScriptableObject
{
    public string ReloadSound => reloadSound;
    [SerializeField] [EventRef] private string reloadSound;

    public string EmptyClipSound => emptyClipSound;
    [SerializeField] [EventRef] private string emptyClipSound;

    public string ShootSound => shootSound;
    [SerializeField] [EventRef] private string shootSound;

    public string EquipSound => equipSound;
    [SerializeField] [EventRef] private string equipSound;

    public string ImpactSound => impactSound;
    [SerializeField] [EventRef] private string impactSound;

    public string InAirLoop => inAirLoop;
    [SerializeField] [EventRef] private string inAirLoop;
}
