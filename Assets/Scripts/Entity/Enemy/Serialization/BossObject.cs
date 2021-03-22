using Rhyth.BTree;
using UnityEngine;

/// <summary>
/// Used for creating an boss entity.
/// </summary>
[CreateAssetMenu(menuName = "Entity/Boss")]
public class BossObject : ScriptableObject
{
    public EnemyObject EnemyObject => enemyObject;
    [SerializeField] private EnemyObject enemyObject;

    public BossEnterAnimation.AnimationType AnimationType => animationType;
    [SerializeField] private BossEnterAnimation.AnimationType animationType;
}
