using Mirror;
using UnityEngine;

public class EnemyTest : NetworkBehaviour
{
    [SerializeField] private EnemyObject enemyObject;

    public override void OnStartServer()
    {
        GameObject enemy = Instantiate(enemyObject.Prefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        enemy.GetComponent<Enemy>().Set(enemyObject);
        NetworkServer.Spawn(enemy);
    }
}
