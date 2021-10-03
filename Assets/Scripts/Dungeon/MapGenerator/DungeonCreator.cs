using MapGenerator;
using System.Collections;
using System.Collections.Generic;
using TiledSharp;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonCreator : MonoBehaviour
{
    public static DungeonCreator Instance { get; private set; }

    [HideInInspector]
    public Dungeon dungeon;

    [Header("Tilemaps")]
    [SerializeField]
    private Tilemap tilemapFloor;
    [SerializeField]
    private Tilemap tilemapWall;
    [SerializeField]
    private Tilemap tilemapCeiling;

    [SerializeField]
    private Tile tilePlaceHolder;
    [SerializeField]
    private Tile tilePlaceHolderHalf;

    [SerializeField]
    private Transform objectContainer;
    public Transform RoomsContainer => roomsContainer;
    [SerializeField]
    private Transform roomsContainer;

    public Transform CorridorContainer => corridorContainer;
    [SerializeField]
    private Transform corridorContainer;

    public GameObject PrefabDungeonRoom => prefabDungeonRoom;
    [SerializeField]
    private GameObject prefabDungeonRoom;

    [SerializeField]
    private Transform mask;

    [SerializeField]
    private RoomCreator roomCreator;

    [Header("Settings")]
    [SerializeField]
    private Vector2Int maxSize = Vector2Int.one;

    private DungeonTimer timer;
    private Vector3 position;

    #region UnityEvents
    private void Awake()
    {
        if (Instance)
        {
            UnityEngine.Debug.LogWarning("Already a DungeonCreator in scene! Deleting myself!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
    #endregion

    /// <summary>
    /// Creates the dungeon in an async way.
    /// </summary>
    /// <param name="seed">The random seed for the dungeon.</param>
    /// <param name="levelNumber">The current level number.</param>
    /// <param name="config">The config to load and use during generation.</param>
    /// <param name="tileset">The tileset to use for the tilemap generation.</param>
    public IEnumerator CreateDungeon(int seed, int levelNumber, DungeonConfig config, Tileset tileset, Dungeon dungeonToLoad = null)
    {
        timer = new DungeonTimer();
        timer.Start();

        yield return DestroyPreviousGameObjects();

        dungeon = dungeonToLoad;
        if (dungeon == null)
        {
            Debug.Log("Creating dungeon with seed: " + seed);
            // Load all room types
            List<Fast2DArray<int>> roomLayouts = new List<Fast2DArray<int>>();
            List<List<TiledImporter.PrefabLocations>> roomGameObjects = new List<List<TiledImporter.PrefabLocations>>();
            List<RoomType> roomTypes = new List<RoomType>();
            yield return LoadRoomTypes(roomLayouts, roomGameObjects, roomTypes);

            //seed = -1304249244;
            seed = "I wanna go home".GetHashCode();
            dungeon = new Dungeon(roomLayouts.ToArray(), roomGameObjects.ToArray(), roomTypes.ToArray(), seed, config);
        }
        DungeonDict.Instance.dungeon = dungeon;

        AdjustMask();

        yield return ClearPreviousTiles();

        yield return SetTiles(tileset);

        yield return SetBorderTiles(tileset);

        yield return roomCreator.CreateRooms(dungeon, levelNumber, timer);

        MiniMapManager.Instance.OnNewLevelGenerated();

        SetLoadStatus(1.0f);

        position = transform.position;

        if (Player.LocalPlayer) // check to allow for debugging if a localplayer is not scene
            Player.LocalPlayer.StateCommunicator.LevelSetLoaded(true);
    }

    /// <summary>
    /// Clears a previous dungeon and removes all tiles and objects.
    /// </summary>
    /// <returns></returns>
    public IEnumerator ClearPreviousDungeon()
    {
        timer = new DungeonTimer(false);
        timer.Start();

        yield return DestroyPreviousGameObjects();
        yield return ClearPreviousTiles();

        dungeon = DungeonDict.Instance.dungeon = null;
    }

    #region DungeonCreateHelperMethods
    /// <summary>
    /// Destroys all dungeon gameobjects.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DestroyPreviousGameObjects()
    {
        if (roomsContainer.childCount > 0)
        {
            for (int i = roomsContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(roomsContainer.GetChild(i).gameObject);
            }
        }

        if (corridorContainer.childCount > 0)
        {
            for (int i = corridorContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(corridorContainer.GetChild(i).gameObject);
            }
        }

        if (timer.ShouldWait())
            yield return timer.Wait(0.01f);
    }

    /// <summary>
    /// Loads the roomlayouts, objects and types from the rooms that were created in Tiled.
    /// </summary>
    private IEnumerator LoadRoomTypes(List<Fast2DArray<int>> roomLayouts, List<List<TiledImporter.PrefabLocations>> roomGameObjects, List<RoomType> roomTypes)
    {
        roomLayouts.Add(TiledImporter.Instance.GetReplacableMap("startRoom", out PropertyDict properties, out List<TiledImporter.PrefabLocations> gos));
        if (properties.TryGetValue("roomType", out string value) == false)
            throw new System.Exception("No room type in map: startRoom!");
        if (int.TryParse(value, out int roomType) == false)
            throw new System.Exception("Room type is not an integer in: startRoom!");
        roomTypes.Add((RoomType)roomType);
        roomGameObjects.Add(gos);

        roomLayouts.Add(TiledImporter.Instance.GetReplacableMap("bossRoom", out properties, out gos));
        if (properties.TryGetValue("roomType", out value) == false)
            throw new System.Exception("No room type in map: bossRoom!");
        if (int.TryParse(value, out roomType) == false)
            throw new System.Exception("Room type is not an integer in: bossRoom!");
        roomTypes.Add((RoomType)roomType);
        roomGameObjects.Add(gos);

        roomLayouts.Add(TiledImporter.Instance.GetReplacableMap("shopRoom", out properties, out gos));
        if (properties.TryGetValue("roomType", out value) == false)
            throw new System.Exception("No room type in map: shopRoom!");
        if (int.TryParse(value, out roomType) == false)
            throw new System.Exception("Room type is not an integer in: shopRoom!");
        roomTypes.Add((RoomType)roomType);
        roomGameObjects.Add(gos);

        int mapCount = TiledDict.Instance.TileMapCount - 3;
        for (int i = 1; i <= mapCount; i++)
        {
            roomLayouts.Add(TiledImporter.Instance.GetReplacableMap("room" + i.ToString(), out properties, out gos));

            if (properties.TryGetValue("roomType", out value) == false)
                throw new System.Exception("No room type in map: room" + i + "!");

            if (int.TryParse(value, out roomType) == false)
                throw new System.Exception("Room type is not an integer in: room" + i + "!");

            roomTypes.Add((RoomType)roomType);

            roomGameObjects.Add(gos);
        }

        if (timer.ShouldWait())
            yield return timer.Wait(0.05f);
    }

    /// <summary>
    /// Adjusts the mask to make the whole dungeon visible.
    /// </summary>
    private void AdjustMask()
    {
        mask.localScale = new Vector3(dungeon.Size.x, dungeon.Size.y, 1f);
        mask.position = transform.position + (mask.localScale / 2f);
    }

    /// <summary>
    /// Clears all tilemaps.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ClearPreviousTiles()
    {
        tilemapFloor.ClearAllTiles();

        if (timer.ShouldWait())
            yield return timer.Wait(0.1f);

        tilemapWall.ClearAllTiles();
        tilemapCeiling.ClearAllTiles();

        if (timer.ShouldWait())
            yield return timer.Wait(0.1f);
    }

    /// <summary>
    /// Sets the tiles of the tilemap asynchronously due to performance reasons.
    /// </summary>
    /// <param name="tileset">The tileset to use.</param>
    /// <returns></returns>
    private IEnumerator SetTiles(Tileset tileset)
    {
        // create new tilemaps
        Vector3Int[] positionsFloor;
        Vector3Int[] positionsWall;
        Vector3Int[] positionsCeiling;
        TileBase[] tilesFloor;
        TileBase[] tilesWall;
        TileBase[] tilesCeiling;

        int indexFloor;
        int indexWall;
        int indexCeil;

        for (int x = 0; x < dungeon.Size.x; x++)
        {
            positionsFloor = new Vector3Int[dungeon.Size.y];
            positionsWall = new Vector3Int[dungeon.Size.y];
            positionsCeiling = new Vector3Int[dungeon.Size.y];
            tilesFloor = new TileBase[dungeon.Size.y];
            tilesWall = new TileBase[dungeon.Size.y];
            tilesCeiling = new TileBase[dungeon.Size.y];
            indexFloor = 0;
            indexWall = 0;
            indexCeil = 0;

            for (int y = 0; y < dungeon.Size.y; y++)
            {
                positionsFloor[indexFloor] = new Vector3Int(x, y, 0);
                if (dungeon[x, y] == TileType.Floor && dungeon[x, y - 1] == TileType.Floor && dungeon[x, y - 2] == TileType.Floor)
                    tilesFloor[indexFloor] = tileset.tileFloor;
                else if (dungeon[x, y] == TileType.Floor && dungeon[x, y - 1] == TileType.Floor)
                    tilesFloor[indexFloor] = tilePlaceHolderHalf;
                else
                    tilesFloor[indexFloor] = tilePlaceHolder;
                indexFloor++;

                if (y >= 1 && dungeon[x, y] == TileType.Wall && dungeon[x, y - 1] == TileType.Floor)
                {
                    positionsWall[indexWall] = new Vector3Int(x, y, 0);
                    tilesWall[indexWall] = tileset.tileWall;

                    indexWall++;
                }

                if (y >= 2 && dungeon[x, y - 2] == TileType.Wall)
                {
                    positionsCeiling[indexCeil] = new Vector3Int(x, y, 0);
                    tilesCeiling[indexCeil] = tileset.tileCeiling;

                    indexCeil++;
                }
            }

            // set tiles
            tilemapFloor.SetTiles(positionsFloor, tilesFloor);
            tilemapWall.SetTiles(positionsWall, tilesWall);
            tilemapCeiling.SetTiles(positionsCeiling, tilesCeiling);

            // see if we can do another iteration or wait until next frame so the game does not "hang"
            if (timer.ShouldWait())
                yield return timer.Wait(0.1f + ((x * 1.0f / dungeon.Size.x) * 0.85f));
        }
    }

    /// <summary>
    /// Sets the overlapping border tiles, so the player cannot look behind the tilemap.
    /// </summary>
    /// <param name="tileset"></param>
    /// <returns></returns>
    private IEnumerator SetBorderTiles(Tileset tileset)
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        List<TileBase> tiles = new List<TileBase>();
        yield return InnerSetBorderTiles(positions, tiles, tileset, -10, dungeon.Size.x + 10, -10, 2);
        yield return InnerSetBorderTiles(positions, tiles, tileset, -10, dungeon.Size.x + 10, dungeon.Size.y, dungeon.Size.y + 10);
        yield return InnerSetBorderTiles(positions, tiles, tileset, -10, 0, 0, dungeon.Size.y);
        yield return InnerSetBorderTiles(positions, tiles, tileset, dungeon.Size.x, dungeon.Size.x + 10, 0, dungeon.Size.y);
    }

    private IEnumerator InnerSetBorderTiles(List<Vector3Int> positions, List<TileBase> tiles, Tileset tileset, int xStart, int xEnd, int yStart, int yEnd)
    {
        for (int x = xStart; x < xEnd; x++)
        {
            for (int y = yStart; y < yEnd; y++)
            {
                positions.Add(new Vector3Int(x, y, 0));
                tiles.Add(tileset.tileCeiling);
            }
        }
        tilemapCeiling.SetTiles(positions.ToArray(), tiles.ToArray());
        positions.Clear();
        tiles.Clear();

        if (timer.ShouldWait())
            yield return timer.Wait(0.95f);
    }
    #endregion

    #region PublicMethods
    /// <summary>
    /// Creates a new dungeon.
    /// </summary>
    /// <param name="levelNumber"></param>
    public void CreateLevel(int levelNumber)
    {
        StartCoroutine(CreateDungeon(GameManager.gameMode.levelSeeds[levelNumber], levelNumber, GameManager.gameMode.dungeonConfig, RegionDict.Instance.Tileset));
    }

    /// <summary>
    /// Adjusts the mask to given position and scale.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="scale"></param>
    public void AdjustMask(Vector3 position, Vector3 scale)
    {
        mask.localScale = scale;
        mask.position = position + scale / 2f + new Vector3(0f, 1f, 0f);
    }

    /// <summary>
    /// Resets the mask to the full dungeon size.
    /// </summary>
    public void ResetMask()
    {
        mask.localScale = new Vector3(dungeon.Size.x, dungeon.Size.y, 1f);
        mask.position = transform.position + (mask.localScale / 2f);
    }

    /// <summary>
    /// Calculates the world position from a tile and adding half.
    /// </summary>
    public Vector3 TilePositionToWorldPositionMiddle(Vector2Int pos)
    {
        return position + new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0.0f);
        //return tilemapFloor.CellToWorld((Vector3Int)pos) + tilemapFloor.cellSize * 0.5f;
    }

    /// <summary>
    /// Calculates the world position from a tile and adding half.
    /// </summary>
    public Vector3 TilePositionToWorldPositionMiddle(int x, int y)
    {
        return position + new Vector3(x + 0.5f, y + 0.5f, 0.0f);
        // return tilemapFloor.CellToWorld(new Vector3Int(x, y, 0)) + tilemapFloor.cellSize * 0.5f;
    }

    /// <summary>
    /// Calculates the world position from a tile.
    /// </summary>
    public Vector3 TilePositionToWorldPosition(Vector2Int pos)
    {
        return position + new Vector3(pos.x, pos.y, 0.0f);
        // return tilemapFloor.CellToWorld((Vector3Int)pos);
    }

    /// <summary>
    /// Calculates the tile position from a worldposition.
    /// </summary>
    public Vector2Int WorldPositionToTilePosition(Vector3 pos)
    {
        return new Vector2Int((int)(pos.x - position.x), (int)(pos.y - position.y));
        // return (Vector2Int)tilemapFloor.WorldToCell(pos);
    }

    /// <summary>
    /// Sets the current load status of the map (used in loading screen).
    /// </summary>
    public void SetLoadStatus(float currentLoadStatus)
    {
        if (Player.LocalPlayer)
            Player.LocalPlayer.StateCommunicator.SetLevelLoadPercentage(currentLoadStatus);
    }
    #endregion
}
