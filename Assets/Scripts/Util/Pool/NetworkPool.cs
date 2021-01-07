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
        CreateInitialPool();

        ClientScene.RegisterPrefab(prefab, Get, Free);
    }

    public override void OnStartClient()
    {
        if (isServer)
            return;

        CreateInitialPool();

        ClientScene.RegisterPrefab(prefab, Get, Free);
    }

    private void CreateInitialPool()
    {
        poolQueue = new Queue<GameObject>(maxCapacity);
        for (int i = 0; i < startingAmount; i++)
        {
            GameObject t = Create();
            t.GetComponent<IPoolable>().Hide();
            poolQueue.Enqueue(t);
        }
    }

    public GameObject Get(SpawnMessage spawnMessage)
    {
        GameObject obj = GetFromPool();
        obj.transform.position = spawnMessage.position;
        obj.transform.rotation = spawnMessage.rotation;
        return obj;
    }

    public GameObject GetFromPool()
    {
        return poolQueue.Count == 0 ? Create() : poolQueue.Dequeue();
    }

    protected abstract GameObject Create();

    public void Free(GameObject t)
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
