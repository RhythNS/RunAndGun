using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapDisplayEverything : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Show every room of minimap with F6");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
            Unlock();
    }

    private void Unlock()
    {
        DungeonRoom[] rooms = DungeonDict.Instance.Rooms;
        
        foreach (DungeonRoom room in rooms)
        {
            MiniMapNewRoomMessage newRoomMessage = new MiniMapNewRoomMessage
            {
                roomId = room.id
            };
            NetworkServer.SendToAll(newRoomMessage);
        }
    }
}
