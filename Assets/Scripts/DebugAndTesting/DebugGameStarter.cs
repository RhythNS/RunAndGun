using MapGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGameStarter : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);

        UIManager.Instance.ShowLevelLoadScreen();
        DungeonDict.Instance.ClearRooms();
        RegionSceneLoader loader = RegionSceneLoader.Instance;
        yield return loader.LoadScene(Region.EnemyTestRoom);

        Vector2Int startPos = new Vector2Int(1, 3);
        Vector2Int startSize = new Vector2Int(10, 10);
        Vector2Int corridorPos = startPos + new Vector2Int(startSize.x - 1, 2);
        Vector2Int corridorSize = new Vector2Int(10, 2);
        Vector2Int enemyPos = startPos + new Vector2Int(startSize.x - 1, 0) + new Vector2Int(corridorSize.x - 1, 0);
        Vector2Int enemySize = new Vector2Int(20, 20);

        Fast2DArray<TileType> layout = new Fast2DArray<TileType>(startSize.x, startSize.y);
        FillRoomWithBounds(layout);
        layout.Set(TileType.CorridorAccess, layout.XSize - 1, 2);
        layout.Set(TileType.CorridorAccess, layout.XSize - 1, 3);

        List<TiledImporter.PrefabLocations> gameObjects = new List<TiledImporter.PrefabLocations>();
        Room startRoom = new Room(startPos.x, startPos.y, layout, gameObjects, RoomType.Start);

        Corridor corridor = new Corridor(corridorPos.x, corridorPos.y, corridorSize.x, Direction.Right);

        layout = new Fast2DArray<TileType>(enemySize.x, enemySize.y);
        FillRoomWithBounds(layout);
        layout.Set(TileType.CorridorAccess, 0, 2);
        layout.Set(TileType.CorridorAccess, 0, 3);

        Room enemyRoom = new Room(enemyPos.x, enemyPos.y, layout, gameObjects, RoomType.Combat);

        Room[] rooms = { startRoom, enemyRoom };
        Corridor[] corridors = { corridor };

        GlobalsDict.Instance.GameStateManagerObject.AddComponent<GameManager>();

        Dungeon dungeon = new Dungeon(rooms, corridors, enemyPos + new Vector2Int(layout.XSize, layout.YSize));
        yield return DungeonCreator.Instance.CreateDungeon(-1, 1, DungeonConfig.StandardConfig, dungeon);

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
