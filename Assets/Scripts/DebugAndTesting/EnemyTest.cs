using Mirror;
using UnityEngine;

public class EnemyTest : NetworkBehaviour
{
    [SerializeField] private GameObject enemyPrefab;

    public override void OnStartServer()
    {
        GameObject enemy = Instantiate(enemyPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        NetworkServer.Spawn(enemy);
    }
}
