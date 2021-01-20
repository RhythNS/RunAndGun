using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public class DungeonDoor : MonoBehaviour
{
    /// <summary>
    /// If the door is left-right or up-down aligned.
    /// </summary>
    public bool IsLeftRight { get; set; } = false;

    private BoxCollider2D coll;

    private bool isLocked = true;
    /// <summary>
    /// Gets the current lockstatus. If the door is open and IsLocked is set to true, the door will close.
    /// </summary>
    public bool IsLocked {
        get => isLocked;
        set {
            if (value && IsOpen)
                Close();

            isLocked = value;
        }
    }

    /// <summary>
    /// If the door is currently open or closed.
    /// </summary>
    public bool IsOpen { get; set; } = false;

    public void Start() {
        coll = GetComponent<BoxCollider2D>();
    }

    /// <summary>
    /// Opens the door. If locked or already open this does nothing.
    /// </summary>
    public void Open() {
        if (IsLocked || IsOpen)
            return;

        coll.enabled = false;
        transform.position += Vector3.up * 2f;
    }

    /// <summary>
    /// Closes the door. If it is already closed this does nothing.
    /// </summary>
    public void Close() {
        if (!IsOpen)
            return;

        coll.enabled = true;
        transform.position += Vector3.down * 2f;
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        Open();
    }
}
