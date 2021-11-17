using MapGenerator;
using UnityEngine;

/// <summary>
/// Dict for data important to the current level.
/// </summary>
public class DungeonDict : MonoBehaviour
{
    public static DungeonDict Instance { get; private set; }

    /// <summary>
    /// All rooms in the dungeon.
    /// </summary>
    public DungeonRoom[] Rooms { get; private set; }
    /// <summary>
    /// A reference to the boss room.
    /// </summary>
    public BossRoom BossRoom { get; private set; }
    /// <summary>
    /// A reference to the dungeon in its generation form.
    /// </summary>
    public Dungeon dungeon;

    public IPathfinder pathfinder;

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
    }

    private void Start()
    {
        if (debug)
        {
            DungeonRoom[] rooms = FindObjectsOfType<DungeonRoom>();
            ResetRooms(rooms.Length);
            for (int i = 0; i < rooms.Length; i++)
            {
                rooms[i].id = i;
                Register(rooms[i]);
            }
        }
    }

    /// <summary>
    /// Registers a new dungeon room to the dict.
    /// </summary>
    /// <param name="room">The room to be added.</param>
    public void Register(DungeonRoom room)
    {
        if (room.id < 0 || room.id >= Rooms.Length)
        {
            Debug.LogError("Room id outside of reseted bounds! id: " + room.id + ", length: " + Rooms.Length);
            return;
        }
        Rooms[room.id] = room;
    }

    /// <summary>
    /// Sets the boss room.
    /// </summary>
    /// <param name="bossRoom">The boss room to be set.</param>
    public void SetBossRoom(BossRoom bossRoom)
    {
        BossRoom = bossRoom;
    }

    /// <summary>
    /// Gets a dungeon room based on its id.
    /// </summary>
    /// <param name="i">The id of the dungeon room.</param>
    /// <returns>A reference to the dungeon room.</returns>
    public DungeonRoom Get(int i) => Rooms[i];

    /// <summary>
    /// Checks if an room id is valid.
    /// </summary>
    /// <param name="id">The id to be checked.</param>
    /// <returns>Wheter it is valid.</returns>
    public bool IsIdValid(int id) => Rooms != null && id > -1 && id < Rooms.Length;

    /// <summary>
    /// Reset the dungeon room array with the new number of dungeon rooms.
    /// </summary>
    /// <param name="numberOfRooms">The new number of dungeon rooms.</param>
    public void ResetRooms(int numberOfRooms)
    {
        Rooms = new DungeonRoom[numberOfRooms];
    }

    /// <summary>
    /// Reset the dungeon rooms list.
    /// </summary>
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
