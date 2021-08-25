using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "Pickable/Weapon/BulletImpact/PlaySoundEffect")]
public class PlaySoundImpactEffect : BulletImpactEffect
{
    [SerializeField] [EventRef] private string fmodEvent;

    public override void OnBulletImpacted(Vector3 position, Health inflicter, bool hitHealth)
    {
        FMODUtil.PlayOnPosition(fmodEvent, position);
    }

}
