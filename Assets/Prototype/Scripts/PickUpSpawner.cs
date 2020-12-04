using System.Collections;
using UnityEngine;

public class PickUpSpawner : MonoBehaviour
{
    [SerializeField] private InstantPickup[] pickups;
    [SerializeField] private float spawnSpeed;
    [SerializeField] private Vector2 halfExtendsSpawnBounds;

    private void Start()
    {
        StartCoroutine(SpawnPickUps());
    }

    private IEnumerator SpawnPickUps()
    {
        while (true)
        {
            Vector3 pos = transform.position;

            Instantiate(pickups[Random.Range(0, pickups.Length)], new Vector3(
                    pos.x + Random.Range(-halfExtendsSpawnBounds.x, halfExtendsSpawnBounds.x),
                    pos.y + Random.Range(-halfExtendsSpawnBounds.y, halfExtendsSpawnBounds.y),
                    pos.z),
                Quaternion.identity);
            yield return new WaitForSeconds(spawnSpeed);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, halfExtendsSpawnBounds * 2);
    }
}
