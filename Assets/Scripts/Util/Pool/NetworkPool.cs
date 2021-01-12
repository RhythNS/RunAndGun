using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NetworkPool : NetworkBehaviour
{
    [SerializeField] protected GameObject prefab;
    [SerializeField] protected int startingAmount;
    [SerializeField] protected int maxCapacity;

    private Queue<GameObject> poolQueue;

    public override void OnStartServer()
    {
        ClientScene.RegisterPrefab(prefab, Get, Free);

        CreateInitialPool();
    }

    public override void OnStartClient()
    {
        if (isServer)
            return;

        ClientScene.RegisterPrefab(prefab, Get, Free);

        CreateInitialPool();
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
        GameObject obj = GetFromPool();
        obj.transform.position = spawnMessage.position;
        obj.transform.rotation = spawnMessage.rotation;
        return obj;
    }

    public virtual GameObject GetFromPool()
    {
        return poolQueue.Count == 0 ? Create() : poolQueue.Dequeue();
    }

    protected abstract GameObject Create();

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
