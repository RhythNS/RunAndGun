using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundDict : MonoBehaviour
{
    public static SoundDict Instance { get; private set; }

    public string ItemPickUpSound => itemPickUpSound;
    [SerializeField] [EventRef] private string itemPickUpSound;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("SoundDict already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
