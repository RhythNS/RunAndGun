using UnityEngine;

/// <summary>
/// Changes the name to a random one from an array.
/// </summary>
public class DebugRandomName : MonoBehaviour
{
    /// <summary>
    /// A random name will be taken from this array.
    /// </summary>
    [SerializeField] private string[] names;

    private void Start()
    {
        Config.Instance.PlayerName = RandomUtil.Element(names);
        Debug.Log("Set random name! Name is now: " + Config.Instance.PlayerName);
    }
}
