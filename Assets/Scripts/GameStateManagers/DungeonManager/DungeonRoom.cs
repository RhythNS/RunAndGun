﻿using System.Collections.Generic;
using UnityEngine;

using MapGenerator;
using Mirror;

public abstract class DungeonRoom : MonoBehaviour
{
    public abstract bool EventOnRoomEntered { get; }

    public abstract RoomType RoomType { get; }

    public bool AlreadyCleared { get; protected set; } = false;

    public Rect Border { get => border; set => border = value; }
    [SerializeField] private Rect border;

    /// <summary>
    /// List of all walkableTiles of the room in worldspace.
    /// </summary>
    public List<Vector2Int> walkableTiles = new List<Vector2Int>();

    /// <summary>
    /// Contains all GameObjects inside a room.
    /// </summary>
    public List<GameObject> objects = new List<GameObject>();

    /// <summary>
    /// Contains all doors of the room.
    /// </summary>
    public List<DungeonDoor> doors = new List<DungeonDoor>();

    /// <summary>
    /// The id of the room.
    /// </summary>
    public int id;

    private void Start()
    {
        RoomDict.Instance.Register(this);
    }

    public bool CheckAllPlayersEntered(List<Bounds> playerBounds)
    {
        for (int i = 0; i < playerBounds.Count; i++)
        {
            if (!Border.Contains(playerBounds[i].min) || !Border.Contains(playerBounds[i].max))
                return false;
        }
        return true;
    }


    public virtual void OnAllPlayersEntered()
    {

    }

    [Server]
    public void CloseDoors()
    {
        DoorMessage doorMessage = new DoorMessage()
        {
            open = false,
            roomId = id
        };

        NetworkServer.SendToAll(doorMessage);
    }

    public void OnCloseDoors()
    {
        foreach (var door in doors)
        {
            door.IsLocked = true;
        }
    }

    [Server]
    public void OpenDoors()
    {
        DoorMessage doorMessage = new DoorMessage()
        {
            open = true,
            roomId = id
        };

        NetworkServer.SendToAll(doorMessage);
    }

    public void OnOpenDoors()
    {
        foreach (var door in doors)
        {
            door.IsLocked = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        float midX = Border.x + Border.width / 2;
        float midY = Border.y + Border.height / 2;
        Gizmos.DrawWireCube(new Vector3(midX, midY), new Vector3(Border.width, Border.height));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        float midX = Border.x + Border.width / 2;
        float midY = Border.y + Border.height / 2;
        Gizmos.DrawWireCube(new Vector3(midX, midY), new Vector3(Border.width, Border.height));
    }
}
