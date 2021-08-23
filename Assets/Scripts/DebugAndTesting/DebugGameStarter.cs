using MapGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Test for starting the game without the need of the lobby.
/// </summary>
public class DebugGameStarter : MonoBehaviour
{
    [SerializeField] private EnemyObject[] enemiesToSpawn;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);

        UIManager.Instance.ShowLevelLoadScreen();
        DungeonDict.Instance.ClearRooms();
        RegionSceneLoader loader = RegionSceneLoader.Instance;
        yield return loader.LoadScene(Region.EnemyTestRoom);

        Vector2Int weaponSize = new Vector2Int(30, 30);
        Vector2Int corridorSize = new Vector2Int(10, 2);
        Vector2Int enemySize = new Vector2Int(20, 20);
        Vector2Int startSize = new Vector2Int(10, 10);

        Vector2Int weaponPos = new Vector2Int(1, 3);
        Vector2Int startPos = new Vector2Int(weaponPos.x + weaponSize.x + corridorSize.x - 2, 3);
        Vector2Int corridor1Pos = startPos + new Vector2Int(0, 2);// - new Vector2Int(corridorSize.x, -2);
        Vector2Int corridor2Pos = startPos + new Vector2Int(startSize.x - 1, 2);
        Vector2Int enemyPos = startPos + new Vector2Int(startSize.x - 1, 0) + new Vector2Int(corridorSize.x - 1, 0);

        Fast2DArray<TileType> layout = new Fast2DArray<TileType>(startSize.x, startSize.y);
        FillRoomWithBounds(layout);
        layout.Set(TileType.CorridorAccess, layout.XSize - 1, 2);
        layout.Set(TileType.CorridorAccess, layout.XSize - 1, 3);

        List<TiledImporter.PrefabLocations> gameObjects = new List<TiledImporter.PrefabLocations>();
        Room startRoom = new Room(startPos.x, startPos.y, layout, gameObjects, RoomType.Start);

        layout = new Fast2DArray<TileType>(weaponSize.x, weaponSize.y);
        FillRoomWithBounds(layout);
        layout.Set(TileType.CorridorAccess, layout.XSize - 1, 2);
        layout.Set(TileType.CorridorAccess, layout.XSize - 1, 3);
        Room weaponRoom = new Room(weaponPos.x, weaponPos.y, layout, gameObjects, RoomType.Empty);

        Corridor corridor1 = new Corridor(corridor1Pos.x, corridor1Pos.y, corridorSize.x, Direction.Left);
        Corridor corridor2 = new Corridor(corridor2Pos.x, corridor2Pos.y, corridorSize.x, Direction.Right);

        layout = new Fast2DArray<TileType>(enemySize.x, enemySize.y);
        FillRoomWithBounds(layout);
        for (int x = 3; x < enemySize.x - 2; x++)
        {
            for (int y = 5; y < enemySize.y - 2; y += 5)
            {
                layout.Set(TileType.Wall, x, y);
            }
        }
        layout.Set(TileType.CorridorAccess, 0, 2);
        layout.Set(TileType.CorridorAccess, 0, 3);

        Room enemyRoom = new Room(enemyPos.x, enemyPos.y, layout, gameObjects, RoomType.Combat);

        Room[] rooms = { startRoom, enemyRoom, weaponRoom };
        Corridor[] corridors = { corridor1, corridor2 };

        if (Player.LocalPlayer.isServer)
            GlobalsDict.Instance.GameStateManagerObject.AddComponent<GameManager>();

        Dungeon dungeon = new Dungeon(rooms, corridors, enemyPos + new Vector2Int(layout.XSize, layout.YSize));
        DungeonConfig config = DungeonConfig.StandardConfig;
        yield return DungeonCreator.Instance.CreateDungeon(-1, 1, config, dungeon);

        CombatRoom combatRoom = DungeonDict.Instance.Rooms.First(x => x.RoomType == RoomType.Combat) as CombatRoom;
        combatRoom.enemiesToSpawn = enemiesToSpawn;

        Destroy(gameObject);
    }

    private void FillRoomWithBounds(Fast2DArray<TileType> layout)
    {
        for (int x = 0; x < layout.XSize; x++)
        {
            for (int y = 0; y < layout.YSize; y++)
            {
                if (x == 0 || x == layout.XSize - 1 || y == 0 || y == layout.YSize - 1)
                    layout.Set(TileType.Wall, x, y);
                else
                    layout.Set(TileType.Floor, x, y);
            }
        }
    }
}
