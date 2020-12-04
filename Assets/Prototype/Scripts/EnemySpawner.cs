using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    struct WeaponCanMove
    {
        public Weapon weapon;
        public bool canMove;
    }

    [SerializeField] private WeaponCanMove[] weapons;
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

            WeaponCanMove wcm = weapons[Random.Range(0, weapons.Length)];

            Instantiate(enemyPrefab, new Vector3(
                    pos.x + Random.Range(-halfExtendsSpawnBounds.x, halfExtendsSpawnBounds.x), 
                    pos.y + Random.Range(-halfExtendsSpawnBounds.y, halfExtendsSpawnBounds.y), 
                    pos.z), 
                Quaternion.identity).Set(wcm.weapon, player, moveSpeed, wcm.canMove);
            yield return new WaitForSeconds(spawnSpeed);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, halfExtendsSpawnBounds * 2);
    }
}
