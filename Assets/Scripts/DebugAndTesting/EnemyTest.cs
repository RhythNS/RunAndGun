using Mirror;
using UnityEngine;

public class EnemyTest : NetworkBehaviour
{
    [SerializeField] private bool shouldSpawn;
    [SerializeField] private EnemyObject enemyObject;

    public override void OnStartServer()
    {
        if (shouldSpawn)
        {
            GameObject enemy = Instantiate(enemyObject.Prefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            enemy.GetComponent<Enemy>().Set(enemyObject, new Rect(-100, -100, 200, 200));
            NetworkServer.Spawn(enemy);
        }
    }
}
