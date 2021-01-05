using UnityEngine;

[CreateAssetMenu(menuName = "Pickable/Weapon/Bullet Info")]
public class BulletInfo : ScriptableObject
{
    public Sprite Sprite => sprite;
    [SerializeField] private Sprite sprite;


}
