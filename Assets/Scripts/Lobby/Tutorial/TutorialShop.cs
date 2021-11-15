using Mirror;
using System.Text;
using UnityEngine;

public class TutorialShop : MonoBehaviour
{
    [SerializeField] private Vector2[] spawnLocations;
    [SerializeField] private Pickable[] toSpawnPickables;
    private PickableInWorld[] spawnedItems;

    private void Start()
    {
        spawnedItems = new PickableInWorld[spawnLocations.Length];

        // Testing:
        int[] toTest = new int[100];
        for (int i = 0; i < toTest.Length; i++)
            toTest[i] = i;
    }

    [Server]
    public void Spawn()
    {
        Pickable[] pickables = RandomUtil.ElementsNoDuplicates(toSpawnPickables, spawnLocations.Length);
        for (int i = 0; i < pickables.Length; i++)
        {
            spawnedItems[i] = PickableInWorld.Place(pickables[i],
                (Vector2)transform.position + spawnLocations[i], true);
        }
    }

    [Server]
    public void DeSpawn()
    {
        if (spawnedItems == null || spawnedItems.Length == 0 || spawnedItems[0] == null)
            return;

        for (int i = 0; i < spawnedItems.Length; i++)
        {
            if (!spawnedItems[i])
                continue;

            NetworkServer.Destroy(spawnedItems[i].gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnLocations == null || spawnLocations.Length == 0)
            return;

        Gizmos.color = Color.red;
        for (int i = 0; i < spawnLocations.Length; i++)
            GizmosUtil.DrawPoint((Vector2)(transform.position) + spawnLocations[i]);
    }
}
