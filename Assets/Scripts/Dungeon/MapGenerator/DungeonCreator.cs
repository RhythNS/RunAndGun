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
    private Tileset tileset;
    [SerializeField]
    private Tile tilePlaceHolder;
    [SerializeField]
    private Tile tilePlaceHolderHalf;

    [SerializeField]
    private Transform objectContainer;
    public Transform RoomsContainer => roomsContainer;
    [SerializeField]
    private Transform roomsContainer;

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

    public IEnumerator CreateDungeon(int seed, int levelNumber, DungeonConfig config)
    {
        Debug.Log("Creating dungeon with seed: " + seed);

        timer = new DungeonTimer();
        timer.Start();

        yield return DestroyPreviousGameObjects();

        // Load all room types
        List<Fast2DArray<int>> roomLayouts = new List<Fast2DArray<int>>();
        List<List<TiledImporter.PrefabLocations>> roomGameObjects = new List<List<TiledImporter.PrefabLocations>>();
        List<RoomType> roomTypes = new List<RoomType>();
        yield return LoadRoomTypes(roomLayouts, roomGameObjects, roomTypes);

        dungeon = new Dungeon(roomLayouts.ToArray(), roomGameObjects.ToArray(), roomTypes.ToArray(), seed, config);

        AdjustMask();

        yield return ClearPreviousTiles();

        yield return SetTiles();

        yield return SetBorderTiles();

        yield return roomCreator.CreateRooms(dungeon, levelNumber, timer);

        SetLoadStatus(1.0f);

        position = transform.position;

        if (Player.LocalPlayer) // check to allow for debugging if a localplayer is not scene
            Player.LocalPlayer.StateCommunicator.CmdLevelSetLoaded(true);
    }

    #region DungeonCreateHelperMethods
    private IEnumerator DestroyPreviousGameObjects()
    {
        if (roomsContainer.childCount > 0)
        {
            for (int i = roomsContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(roomsContainer.GetChild(i).gameObject);
            }
        }

        if (timer.ShouldWait())
            yield return timer.Wait(0.01f);
    }

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

    private void AdjustMask()
    {
        mask.localScale = new Vector3(dungeon.Size.x, dungeon.Size.y, 1f);
        mask.position = transform.position + (mask.localScale / 2f);
    }

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

    private IEnumerator SetTiles()
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

    private IEnumerator SetBorderTiles()
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        List<TileBase> tiles = new List<TileBase>();
        yield return InnerSetBorderTiles(positions, tiles, -10, dungeon.Size.x + 10, -10, 2);
        yield return InnerSetBorderTiles(positions, tiles, -10, dungeon.Size.x + 10, dungeon.Size.y, dungeon.Size.y + 10);
        yield return InnerSetBorderTiles(positions, tiles, -10, 0, 0, dungeon.Size.y);
        yield return InnerSetBorderTiles(positions, tiles, dungeon.Size.x, dungeon.Size.x + 10, 0, dungeon.Size.y);
    }

    private IEnumerator InnerSetBorderTiles(List<Vector3Int> positions, List<TileBase> tiles, int xStart, int xEnd, int yStart, int yEnd)
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
    public void CreateLevel(int levelNumber)
    {
        StartCoroutine(CreateDungeon(GameManager.gameMode.levelSeeds[levelNumber], levelNumber, GameManager.gameMode.dungeonConfig));
    }

    public void AdjustMask(Vector3 position, Vector3 scale)
    {
        mask.localScale = scale + new Vector3(0f, 1.5f, 0f);
        mask.position = position + scale / 2f + new Vector3(0f, 1f, 0f);
    }

    public void ResetMask()
    {
        mask.localScale = new Vector3(dungeon.Size.x, dungeon.Size.y, 1f);
        mask.position = transform.position + (mask.localScale / 2f);
    }

    public Vector3 TilePositionToWorldPositionMiddle(Vector2Int pos)
    {
        return position + new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0.0f);
        //return tilemapFloor.CellToWorld((Vector3Int)pos) + tilemapFloor.cellSize * 0.5f;
    }

    public Vector3 TilePositionToWorldPositionMiddle(int x, int y)
    {
        return position + new Vector3(x + 0.5f, y + 0.5f, 0.0f);
        // return tilemapFloor.CellToWorld(new Vector3Int(x, y, 0)) + tilemapFloor.cellSize * 0.5f;
    }
    public Vector3 TilePositionToWorldPosition(Vector2Int pos)
    {
        return position + new Vector3(pos.x, pos.y, 0.0f);
        // return tilemapFloor.CellToWorld((Vector3Int)pos);
    }

    public Vector2Int WorldPositionToTilePosition(Vector3 pos)
    {
        return new Vector2Int((int)(pos.x - position.x), (int)(pos.y - position.y));
        // return (Vector2Int)tilemapFloor.WorldToCell(pos);
    }

    public void SetLoadStatus(float currentLoadStatus)
    {
        if (Player.LocalPlayer)
            Player.LocalPlayer.StateCommunicator.CmdSetLevelLoadPercentage(currentLoadStatus);
    }
    #endregion
}
