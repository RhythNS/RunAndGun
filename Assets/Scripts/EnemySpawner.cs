using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Weapon weapon;
    [SerializeField] private Player player;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Enemy enemyPrefab;

    [SerializeField] private float spawnSpeed;
    [SerializeField] private Vector2 halfExtendsSpawnBounds;

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while(true)
        {
            Vector3 pos = transform.position;

            Instantiate(enemyPrefab, new Vector3(
                    pos.x + Random.Range(-halfExtendsSpawnBounds.x, halfExtendsSpawnBounds.x), 
                    pos.y + Random.Range(-halfExtendsSpawnBounds.y, halfExtendsSpawnBounds.y), 
                    pos.z), 
                Quaternion.identity).Set(weapon, player, moveSpeed);
            yield return new WaitForSeconds(spawnSpeed);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, halfExtendsSpawnBounds * 2);
    }
}
