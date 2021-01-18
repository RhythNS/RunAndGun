using Mirror;
using System.Collections.Generic;
using UnityEngine;

public abstract class DungeonRoom : NetworkBehaviour
{
    public abstract bool EventOnRoomEntered { get; }

    public bool AlreadyCleared { get; protected set; } = false;

    [SerializeField] protected Rect bounds;

    public override void OnStartServer()
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        float midX = bounds.x + bounds.width / 2;
        float midY = bounds.y + bounds.height / 2;
        Gizmos.DrawWireCube(new Vector3(midX, midY), new Vector3(bounds.width, bounds.height));
    }

}
