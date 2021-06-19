using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPool : MonoBehaviour
{
    [SerializeField] protected GameObject prefab;
    [SerializeField] protected int startingAmount;
    [SerializeField] protected int maxCapacity;

    private Queue<GameObject> poolQueue;

    private void Start()
    {
        CreateInitialPool();

        NetworkClient.RegisterPrefab(prefab, Get, Free);
    }

    protected virtual void CreateInitialPool()
    {
        poolQueue = new Queue<GameObject>(maxCapacity);
        for (int i = 0; i < startingAmount; i++)
        {
            GameObject t = Create();
            t.GetComponent<IPoolable>().Hide();
            poolQueue.Enqueue(t);
        }
    }

    public virtual GameObject Get(SpawnMessage spawnMessage)
    {
        GameObject obj = GetFromPool(spawnMessage.position, spawnMessage.rotation);
        return obj;
    }

    public virtual GameObject GetFromPool(Vector3 position, Quaternion rotation)
    {
        GameObject obj = poolQueue.Count == 0 ? Create() : poolQueue.Dequeue();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.GetComponent<IPoolable>().Show();
        return obj;
    }

    protected virtual GameObject Create()
    {
        return Instantiate(prefab, transform);
    }

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
