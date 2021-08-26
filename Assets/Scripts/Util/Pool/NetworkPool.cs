using Mirror;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages poolable objects that need to be synchronised in a network.
/// </summary>
public class NetworkPool : MonoBehaviour
{
    [SerializeField] protected GameObject prefab;
    [SerializeField] protected int startingAmount;
    [SerializeField] protected int maxCapacity;

    private Queue<GameObject> poolQueue;

    /// <summary>
    /// Registers the prefab for spawning and unspawning and creates the initial
    /// amount of Poolables.
    /// </summary>
    public void Setup()
    {
        CreateInitialPool();
        ClientScene.RegisterPrefab(prefab, Get, Free);
    }

    /// <summary>
    /// Creates the initial amount of poolables.
    /// </summary>
    protected virtual void CreateInitialPool()
    {
        if (poolQueue == null)
            poolQueue = new Queue<GameObject>(maxCapacity);

        if (poolQueue.Count < startingAmount)
        {
            for (int i = 0; i < startingAmount; i++)
            {
                GameObject t = Create();
                t.GetComponent<IPoolable>().Hide();
                poolQueue.Enqueue(t);
            }
            return;
        }

        // pool count is bigger than startingAmount
        while (poolQueue.Count > startingAmount)
        {
            Destroy(poolQueue.Dequeue());
        }
    }

    /// <summary>
    /// Gets a poolable from the pool.
    /// </summary>
    /// <param name="spawnMessage">The spawn message describing how the poolable should spawn.</param>
    /// <returns>A reference to the gameobject that was gotten from the pool.</returns>
    public virtual GameObject Get(SpawnMessage spawnMessage)
    {
        GameObject obj = GetFromPool(spawnMessage.position, spawnMessage.rotation);
        return obj;
    }

    /// <summary>
    /// Gets a poolable from the pool.
    /// </summary>
    /// <param name="position">The position that the poolable should be in.</param>
    /// <param name="rotation">The rotation that the poolable should be in.</param>
    /// <returns>A reference to the gameobject that was gotten from the pool.</returns>
    public virtual GameObject GetFromPool(Vector3 position, Quaternion rotation)
    {
        GameObject obj = poolQueue.Count == 0 ? Create() : poolQueue.Dequeue();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.GetComponent<IPoolable>().Show();
        return obj;
    }

    /// <summary>
    /// Creates a new poolable.
    /// </summary>
    /// <returns>A refernce to the newly created poolable.</returns>
    protected virtual GameObject Create()
    {
        return Instantiate(prefab, transform);
    }

    /// <summary>
    /// Puts the poolable back into trhe pool.
    /// </summary>
    /// <param name="t">The gameobject to be placed back into the pool.</param>
    public virtual void Free(GameObject t)
    {
        if (poolQueue.Count == maxCapacity)
        {
            Debug.LogWarning("More bullets to free than there is capacity!");
            t.GetComponent<IPoolable>().Delete();
            return;
        }
        t.GetComponent<IPoolable>().Hide();
        poolQueue.Enqueue(t);
    }
}
