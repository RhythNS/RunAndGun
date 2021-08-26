using FMODUnity;
using UnityEngine;

/// <summary>
/// Plays a sound when the bullet impacted something.
/// </summary>
[CreateAssetMenu(menuName = "Pickable/Weapon/BulletImpact/PlaySoundEffect")]
public class PlaySoundImpactEffect : BulletImpactEffect
{
    [SerializeField] [EventRef] private string fmodEvent;

    public override void OnBulletImpacted(Vector3 position, Health inflicter, bool hitHealth)
    {
        FMODUtil.PlayOnPosition(fmodEvent, position);
    }
}
