using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablesDict : MonoBehaviour
{
    public static BreakablesDict Instance { get; private set; }

    [SerializeField]
    private Breakable[] breakables;

    public int BreakablesCount {
        get => breakables.Length;
    }

    [System.Serializable]
    public struct Breakable
    {
        public Sprite full;
        public Sprite broken;
    }

    public void Awake() {
        if (Instance) {
            Debug.LogError("BreakablesDict already in scene! Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public Breakable GetBreakable(int index) {
        if (index < 0 || index >= breakables.Length)
            throw new System.IndexOutOfRangeException("Index was not in range of breakables array");

        return breakables[index];
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
