using System.Collections.Generic;
using UnityEngine;

public abstract class Pool<T> : MonoBehaviour where T : IPoolable
{
    [SerializeField] protected T prefab;
    [SerializeField] protected int startingAmount;
    [SerializeField] protected int maxCapacity;

    private Queue<T> poolQueue;

    private void Awake()
    {
        poolQueue = new Queue<T>(maxCapacity);
        for (int i = 0; i < startingAmount; i++)
        {
            T t = Create();
            t.Hide();
            poolQueue.Enqueue(t);
        }

        InnerAwake();
    }

    protected abstract void InnerAwake();

    public T Get()
    {
        if (poolQueue.Count == 0)
            return Create();
        return poolQueue.Dequeue();
    }

    protected abstract T Create();

    public void Free(T t)
    {
        if (poolQueue.Count == maxCapacity)
        {
            Debug.LogWarning("More bullets to free than there is capacity!");
            t.Delete();
            return;
        }
        t.Hide();
        poolQueue.Enqueue(t);
    }
}
