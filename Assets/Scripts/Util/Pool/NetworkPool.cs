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
        poolQueue = new Queue<GameObject>(maxCapacity);
        for (int i = 0; i < startingAmount; i++)
        {
            GameObject t = Create();
            t.GetComponent<IPoolable>().Hide();
            poolQueue.Enqueue(t);
        }
    }

    public GameObject Get()
    {
        if (poolQueue.Count == 0)
            return Create();
        return poolQueue.Dequeue();
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
