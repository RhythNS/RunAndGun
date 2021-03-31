using FMODUnity;

using UnityEngine;

public class DungeonDoor : MonoBehaviour
{
    /// <summary>
    /// If the door is left-right or up-down aligned.
    /// </summary>
    public bool IsLeftRight { get; set; } = false;

    [SerializeField] [EventRef] private string openSound;
    [SerializeField] [EventRef] private string closeSound;

    private BoxCollider2D coll;
    private bool isLocked = false;

    /// <summary>
    /// Gets the current lockstatus. If the door is open and IsLocked is set to true, the door will close.
    /// </summary>
    public bool IsLocked
    {
        get => isLocked;
        set
        {
            if (value && IsOpen)
                Close();

            isLocked = value;
        }
    }

    /// <summary>
    /// If the door is currently open or closed.
    /// </summary>
    public bool IsOpen { get; set; } = false;

    public void Start()
    {
        coll = GetComponent<BoxCollider2D>();
    }

    /// <summary>
    /// Opens the door. If locked or already open this does nothing.
    /// </summary>
    public void Open()
    {
        if (IsLocked || IsOpen)
            return;

        FMODUtil.PlayOnTransform(openSound, transform);

        IsOpen = true;

        coll.enabled = false;
        if (IsLeftRight)
            transform.position += Vector3.up * 3f;
        else
            transform.position += Vector3.up * 2f;
    }

    /// <summary>
    /// Closes the door. If it is already closed this does nothing.
    /// </summary>
    public void Close()
    {
        if (!IsOpen)
            return;

        FMODUtil.PlayOnTransform(closeSound, transform);

        IsOpen = false;

        coll.enabled = true;
        if (IsLeftRight)
            transform.position += Vector3.down * 3f;
        else
            transform.position += Vector3.down * 2f;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Open();
    }
}
