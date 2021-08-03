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

    /// <summary>
    /// Returns the middle of the room as a Vector2. Only works after the border is set!
    /// </summary>
    public Vector2 Middle => border.position + (border.size * 0.5f);

    private BoxCollider2D boxCollider;

    private void Awake()
    {
        boxCollider = gameObject.AddComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;
        gameObject.layer = LayerDict.Instance.GetDungeonRoomLayer();
    }

    /// <summary>
    /// Called when the DungeonCreator has fully created the room.
    /// </summary>
    public virtual void OnFullyCreated() { }

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
        DungeonCreator.Instance.AdjustMask(new Vector2(border.xMin, border.yMin - 1), border.size + new Vector2(0, 1));
    }

    public virtual void OnLocalPlayerLeft()
    {
        //DungeonCreator.Instance.ResetMask();
    }

    public void ForceBorder(Rect rect)
    {
        border = rect;
        boxCollider.offset = rect.position + rect.size * 0.5f;
        boxCollider.size = rect.size;
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
        for (int i = 0; i < doors.Count; i++)
        {
            doors[i].IsLocked = true;
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
        for (int i = 0; i < doors.Count; i++)
        {
            doors[i].IsLocked = false;
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

        Vector2Int playerPos = DungeonCreator.Instance.WorldPositionToTilePosition(Player.LocalPlayer.transform.position);

        while (enemySpawns.Count < enemiesToSpawn.Length && iterations < maxIterations)
        {
            int rnd = Random.Range(0, walkableTiles.Count);
            Vector2Int pos = walkableTiles[rnd];

            bool found = false;
            for (int x = -10; x < 10; x++)
            {
                for (int y = -10; y < 10; y++)
                {
                    Vector2Int p = pos + new Vector2Int(x, y);
                    if (x >= -2 && y >= -2 && x <= 2 && y <= 2)
                    {
                        if (enemySpawns.Contains(p))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (playerPos == p)
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                    break;
            }

            if (!found)
            {
                Enemy.InstantiateAndSpawn(enemiesToSpawn[enemySpawns.Count], Border, new Vector3(pos.x, pos.y, 0f), Quaternion.identity);

                enemySpawns.Add(pos);
            }

            iterations++;
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

        Vector2Int playerPos = DungeonCreator.Instance.WorldPositionToTilePosition(Player.LocalPlayer.transform.position);

        while (lootSpawns.Count < pickables.Length && iterations < maxIterations)
        {
            int rnd = Random.Range(0, walkableTiles.Count);
            Vector2Int pos = walkableTiles[rnd];

            bool found = false;
            for (int x = -5; x < 5; x++)
            {
                for (int y = -5; y < 5; y++)
                {
                    Vector2Int p = pos + new Vector2Int(x, y);
                    if (x >= -2 && y >= -2 && x <= 2 && y <= 2)
                    {
                        if (lootSpawns.Contains(p))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (playerPos == p)
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                    break;
            }

            if (!found)
            {
                PickableInWorld.Place(pickables[lootSpawns.Count], new Vector3(pos.x, pos.y, 0f));

                lootSpawns.Add(pos);
            }

            iterations++;
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
        GizmosUtil.DrawPoint(Middle);
    }
}
