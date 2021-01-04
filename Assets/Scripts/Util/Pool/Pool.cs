using System.Collections.Generic;
using UnityEngine;

public abstract class Pool<T> : MonoBehaviour where T : IPoolable
{
    [SerializeField] private int startingAmount;
    [SerializeField] private int maxCapacity;

    private Queue<T> poolQueue;

    private void Awake()
    {
        poolQueue = new Queue<T>(maxCapacity);
    }

    public T Get()
    {
        if (poolQueue.Count == 0)
            return Create();
        return poolQueue.Dequeue();
    }

    public void Free(T t)
    {
        if (poolQueue.Count == maxCapacity)
        {
            Debug.LogWarning("More bullets to free than there is capacity!");
            t.Delete();
            return;
        }
        t.Reset();
        poolQueue.Enqueue(t);
    }

    protected abstract T Create();
}
