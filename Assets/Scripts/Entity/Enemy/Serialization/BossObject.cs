using Rhyth.BTree;
using UnityEngine;

/// <summary>
/// Used for creating an boss entity.
/// </summary>
[CreateAssetMenu(menuName = "Entity/Boss")]
public class BossObject : ScriptableObject
{
    public GameObject Prefab => prefab;
    [SerializeField] private GameObject prefab;

    public Weapon Weapon => weapon;
    [SerializeField] private Weapon weapon;

    public BTree BehaviourTree => behaviourTree;
    [SerializeField] private BTree behaviourTree;

    public EnemyStats Stats => stats;
    [SerializeField] private EnemyStats stats;

    public BossEnterAnimation.AnimationType AnimationType => animationType;
    [SerializeField] private BossEnterAnimation.AnimationType animationType;
}
