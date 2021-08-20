using System.Collections;
using UnityEngine;

/// <summary>
/// Test class for testing the ExtendedCoroutine class.
/// </summary>
public class CoroutineTest : MonoBehaviour
{
    private ExtendedCoroutine ext;

    private void Awake()
    {
        ext = new ExtendedCoroutine(this, Test(), OnFinished);
        ext.Start();
    }

    private void OnFinished()
    {
        Debug.Log("yay");
    }

    private IEnumerator Test()
    {
        yield return new WaitForSeconds(1.0f);
    }

    private void Update()
    {
        if (ext.IsFinshed)
            Debug.Log("Double yay");
    }
}
