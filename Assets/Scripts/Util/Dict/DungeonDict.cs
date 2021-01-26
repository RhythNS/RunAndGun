using MapGenerator;
using UnityEngine;

public class DungeonDict : MonoBehaviour
{
    public static DungeonDict Instance { get; private set; }

    public DungeonRoom[] Rooms { get; private set; }
    
    public Dungeon dungeon;

    [SerializeField] private bool debug = false;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("RoomDict already in scene! Deleting myself");
            Destroy(this);
            return;
        }
        Instance = this;

        if (debug)
            ResetRooms(100);
    }

    public void Register(DungeonRoom room)
    {
        if (room.id < 0 || room.id >= Rooms.Length)
        {
            Debug.LogError("Room id outside of reseted bounds! id: " + room.id + ", length: " + Rooms.Length);
            return;
        }
        Rooms[room.id] = room;
    }

    public DungeonRoom Get(int i) => Rooms[i];

    public bool IsIdValid(int id) => Rooms != null && id > -1 && id < Rooms.Length;

    public void ResetRooms(int numberOfRooms)
    {
        Rooms = new DungeonRoom[numberOfRooms];
    }

    public void ClearRooms()
    {
        Rooms = null;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
