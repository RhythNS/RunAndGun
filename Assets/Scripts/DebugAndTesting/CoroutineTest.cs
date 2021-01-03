using System.Collections;
using UnityEngine;

public class CoroutineTest : MonoBehaviour
{
    private ExtendedCoroutine ext;

    private void Awake()
    {
        ext = new ExtendedCoroutine(Test(), OnFinished);
        StartCoroutine(ext);
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
