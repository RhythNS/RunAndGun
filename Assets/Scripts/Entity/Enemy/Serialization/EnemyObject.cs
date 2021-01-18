using Rhyth.BTree;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy")]
public class EnemyObject : ScriptableObject
{
    public GameObject Prefab => prefab;
    [SerializeField] private GameObject prefab;

    public Weapon Weapon => weapon;
    [SerializeField] private Weapon weapon;

    public BTree BehaviourTree => behaviourTree;
    [SerializeField] private BTree behaviourTree;

    public EnemyStats Stats => stats;
    [SerializeField] private EnemyStats stats;
}
