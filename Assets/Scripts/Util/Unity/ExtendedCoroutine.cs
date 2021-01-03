using System;
using System.Collections;

public class ExtendedCoroutine : IEnumerator
{
    public bool IsFinshed { get; private set; } = false;
    public object Current => enumerator.Current;

    private readonly IEnumerator enumerator;
    private readonly Action onFinished;

    public ExtendedCoroutine(IEnumerator enumerator, Action onFinished = null)
    {
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
}
