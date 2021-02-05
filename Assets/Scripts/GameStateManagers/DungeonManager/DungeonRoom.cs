using System.Collections.Generic;
using UnityEngine;
using MapGenerator;
using Mirror;

public abstract class DungeonRoom : MonoBehaviour
{
    /// <summary>
    /// Wheter the room has an event if all players entered it.
    /// </summary>
    public abstract bool EventOnRoomEntered { get; }

    public abstract RoomType RoomType { get; }

    /// <summary>
    /// If the room event has already been cleared.
    /// </summary>
    public bool AlreadyCleared { get; protected set; } = false;

    /// <summary>
    /// A border around the room. This is used to determine when the players are fully in a room.
    /// </summary>
    public Rect Border
    {
        get => border; set
        {
            border = value;
            boxCollider.offset = value.position + (value.size * 0.5f);
            boxCollider.size = value.size - new Vector2(3, 3); // TODO: Temp placeholder value for shrinking the bounds.
        }
    }
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

    private BoxCollider2D boxCollider;

    private void Awake()
    {
        boxCollider = gameObject.AddComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;
        gameObject.layer = LayerDict.Instance.GetDungeonRoomLayer();
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

    public virtual void OnLocalPlayerEntered()
    {
        DungeonCreator.Instance.AdjustMask(new Vector2(border.xMin, border.yMin), border.size);
    }

    public virtual void OnLocalPlayerLeft()
    {
        DungeonCreator.Instance.ResetMask();
    }

    /// <summary>
    /// Called when the room has a room event and all players entered the room. Overwrite this to get
    /// custom behaviour. Make sure to call GameManager.OnRoomEventStarted with the border of the room.
    /// </summary>
    public virtual void OnAllPlayersEntered() { }

    /// <summary>
    /// Sends a message to all connected clients to close the doors of this room.
    /// </summary>
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

    /// <summary>
    /// Called when the client recieved a message to close the doors. Should not be called
    /// manually, use CloseDoors() instead.
    /// </summary>
    public void OnCloseDoors()
    {
        foreach (var door in doors)
        {
            door.IsLocked = true;
        }
    }

    /// <summary>
    /// Sends a message to all connected clients to open the doors of this room.
    /// </summary>
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

    /// <summary>
    /// Called when the client recieved a message to open the doors. Should not be called
    /// manually, use OpenDoors() instead.
    /// </summary>
    public void OnOpenDoors()
    {
        foreach (var door in doors)
        {
            door.IsLocked = false;
        }
    }

    /// <summary>
    /// Spawns given enemies inside the walkable tiles in the room.
    /// </summary>
    /// <param name="enemiesToSpawn">The enemies to be spawned.</param>
    protected void SpawnEnemies(EnemyObject[] enemiesToSpawn)
    {
        List<Vector2Int> enemySpawns = new List<Vector2Int>();

        int maxIterations = enemiesToSpawn.Length * 25;
        int iterations = 0;

        while (enemySpawns.Count < enemiesToSpawn.Length)
        {
            int rnd = Random.Range(0, walkableTiles.Count);
            Vector2Int pos = walkableTiles[rnd];

            bool found = false;
            for (int x = -2; x < 2; x++)
            {
                for (int y = -2; y < 2; y++)
                {
                    if (enemySpawns.Contains(new Vector2Int(x, y)))
                    {
                        found = true;
                        break;
                    }
                }
            }

            if (!found)
            {
                Enemy.InstantiateAndSpawn(enemiesToSpawn[enemySpawns.Count], Border, new Vector3(pos.x, pos.y, 0f), Quaternion.identity);
                
                enemySpawns.Add(pos);
            }

            iterations++;
            if (iterations >= maxIterations)
                break;
        }
    }

    /// <summary>
    /// Spawns given loot inside the walkable tiles in the room.
    /// </summary>
    /// <param name="pickables">The pickables to be spawned.</param>
    protected void SpawnLoot(Pickable[] pickables)
    {
        List<Vector2Int> lootSpawns = new List<Vector2Int>();

        int maxIterations = pickables.Length * 25;
        int iterations = 0;

        while (lootSpawns.Count < pickables.Length)
        {
            int rnd = Random.Range(0, walkableTiles.Count);
            Vector2Int pos = walkableTiles[rnd];

            bool found = false;
            for (int x = -2; x < 2; x++)
            {
                for (int y = -2; y < 2; y++)
                {
                    if (lootSpawns.Contains(new Vector2Int(x, y)))
                    {
                        found = true;
                        break;
                    }
                }
            }

            if (!found)
            {
                PickableInWorld.Place(pickables[lootSpawns.Count], new Vector3(pos.x, pos.y, 0f));

                lootSpawns.Add(pos);
            }

            iterations++;
            if (iterations >= maxIterations)
                break;
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
