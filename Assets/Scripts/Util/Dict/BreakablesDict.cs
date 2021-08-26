using UnityEngine;

/// <summary>
/// Dict for all kinds of breakables that can be found in a dungeon.
/// </summary>
public class BreakablesDict : MonoBehaviour
{
    public static BreakablesDict Instance { get; private set; }

    [SerializeField] private Breakable[] breakables;

    /// <summary>
    /// The amount of breakables in the breakable array.
    /// </summary>
    public int BreakablesCount { get => breakables.Length; }

    [System.Serializable]
    public struct Breakable
    {
        public Sprite full;
        public Sprite broken;
    }

    public void Awake()
    {
        if (Instance)
        {
            Debug.LogError("BreakablesDict already in scene! Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Gets a breakable of the given index.
    /// </summary>
    /// <param name="index">The index of the breakable to be gotten.</param>
    /// <returns>A reference to the breakable.</returns>
    public Breakable GetBreakable(int index)
    {
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
