using Mirror;
using UnityEngine;

public class MiniMapDisplayEverything : MonoBehaviour
{
    [SerializeField] [Range(0.1f, 10.0f)] private float zoomLevel = 0.1f;

    private void Start()
    {
        Debug.Log("Show every room of minimap with F6");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
            Unlock();
    }

    private void OnValidate()
    {
        if (MiniMapManager.Instance)
            MiniMapManager.Instance.SetZoomLevel(zoomLevel);
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