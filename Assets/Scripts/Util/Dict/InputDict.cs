using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputDict : MonoBehaviour
{
    public static InputDict Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("InputDict already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public Texture2D MouseCursor => mouseCursor;
    [SerializeField] private Texture2D mouseCursor;

    public MobileUIManager MobileUIManagerPrefab => mobileUIManagerPrefab;
    [SerializeField] private MobileUIManager mobileUIManagerPrefab;

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
