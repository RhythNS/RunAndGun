using System;
using System.Collections;
using UnityEngine;

public class ExtendedCoroutine : IEnumerator
{
    public bool IsFinshed { get; private set; } = false;
    public object Current => enumerator.Current;
    public Coroutine Coroutine { get; private set; }

    private readonly IEnumerator enumerator;
    private readonly Action onFinished;
    private readonly MonoBehaviour onScript;

    public ExtendedCoroutine(MonoBehaviour onScript, IEnumerator enumerator, Action onFinished = null)
    {
        this.onScript = onScript;
        this.enumerator = enumerator;
        this.onFinished = onFinished;
    }

    public bool MoveNext()
    {
        if (enumerator.MoveNext())
            return true;

        IsFinshed = true;
        if (onFinished != null)
            onFinished.Invoke();

        return false;
    }

    public void Reset()
    {
        enumerator.Reset();
    }

    public void Start()
    {
        Coroutine = onScript.StartCoroutine(this);
    }

    public void Stop(bool invokeOnFinished = true)
    {
        if (Coroutine != null)
            onScript.StopCoroutine(Coroutine);
        if (invokeOnFinished && onFinished != null)
            onFinished.Invoke();
    }
}
