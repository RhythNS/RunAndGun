using System.Collections.Generic;
using UnityEngine;

public abstract class DungeonRoom : MonoBehaviour
{
    public abstract bool EventOnRoomEntered { get; }

    public bool AlreadyCleared { get; protected set; } = false;

    [SerializeField] protected Rect bounds;

    private void Start()
    {
        GameManager.RegisterRoom(this);
    }

    public bool CheckAllPlayersEntered(List<Bounds> playerBounds)
    {
        for (int i = 0; i < playerBounds.Count; i++)
        {
            if (!bounds.Contains(playerBounds[i].min) || !bounds.Contains(playerBounds[i].max))
                return false;
        }
        return true;
    }


    public virtual void OnAllPlayersEntered()
    {

    }

    protected void CloseDoors()
    {

    }

    protected void OpenDoors()
    {

    }

}
