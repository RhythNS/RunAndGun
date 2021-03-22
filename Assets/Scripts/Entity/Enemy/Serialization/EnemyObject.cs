using Rhyth.BTree;
using UnityEngine;

/// <summary>
/// Used for creating an enemy entity.
/// </summary>
[CreateAssetMenu(menuName = "Entity/Enemy")]
public class EnemyObject : ScriptableObject
{
    public GameObject Prefab => prefab;
    [SerializeField] private GameObject prefab;

    public Weapon[] Weapons => weapons;
    [SerializeField] private Weapon[] weapons;

    public BTree BehaviourTree => behaviourTree;
    [SerializeField] private BTree behaviourTree;

    public EnemyStats Stats => stats;
    [SerializeField] private EnemyStats stats;
}
