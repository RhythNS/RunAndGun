using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator
{
    /// <summary>
    /// Generates DungeonRooms for generated Dungeons and Corridors.
    /// </summary>
    public class RoomCreator : MonoBehaviour
    {
        /// <summary>
        /// Create and place and DungeonRooms and Corridors from the generated dungeons.
        /// </summary>
        /// <param name="dungeon">The dungeon that was just generated.</param>
        /// <param name="levelNumber">The number of the level.</param>
        /// <param name="timer">The current used DungeonTimer.</param>
        /// <returns></returns>
        public IEnumerator CreateRooms(Dungeon dungeon, int levelNumber, DungeonTimer timer)
        {
            List<DungeonRoom> dungeonRooms = new List<DungeonRoom>();
            DungeonDict.Instance.ResetRooms(dungeon.Rooms.Length + dungeon.Corridors.Length);

            GameObject prefabDoorLR = RegionDict.Instance.PrefabDoorLR;
            GameObject prefabDoorUD = RegionDict.Instance.PrefabDoorUD;

            // Create DungeonRooms
            for (int i = 0; i < dungeon.Rooms.Length; i++)
            {
                GameObject dungeonRoomObject = Instantiate(DungeonCreator.Instance.PrefabDungeonRoom);
                dungeonRoomObject.transform.parent = DungeonCreator.Instance.RoomsContainer;

                DungeonRoom dungeonRoom = SetRoomType(dungeon.Rooms[i], dungeonRoomObject, levelNumber);
                // set room id
                dungeonRoom.id = i;

                DungeonDict.Instance.Register(dungeonRoom);

                SetRoomBorder(dungeon, i, dungeonRoom);
                SetRoomObjects(dungeon.Rooms[i], dungeonRoomObject, dungeonRoom);
                SetDoors(dungeon, prefabDoorLR, prefabDoorUD, i, dungeonRoomObject, dungeonRoom);

                dungeonRooms.Add(dungeonRoom);
                dungeonRoom.OnFullyCreated();

                if (timer.ShouldWait())
                    yield return timer.Wait(0.9f + ((i * 1.0f / dungeon.Rooms.Length) * 0.05f));
            }
            // Create corridors.
            for (int i = 0; i < dungeon.Corridors.Length; i++)
            {
                Corridor corridor = dungeon.Corridors[i];
                GameObject corridorObject = new GameObject("Corridor_" + i);
                corridorObject.transform.parent = DungeonCreator.Instance.CorridorContainer;

                CorridorRoom corridorRoom = corridorObject.AddComponent<CorridorRoom>();
                corridorRoom.id = dungeon.Rooms.Length + i;
                corridorRoom.walkableTiles = corridor.GetWalkableTiles();
                corridorRoom.direction = corridor.Size.y == 3;
                SetCorridorBorder(corridor, corridorRoom);
                DungeonDict.Instance.Register(corridorRoom);
            }
        }

        /// <summary>
        /// Generates specific values for the DungeonType.
        /// </summary>
        /// <param name="room">The current room.</param>
        /// <param name="dungeonRoomObject">The gameobject to which the room should be generate don.</param>
        /// <param name="levelNumber">The number of the level.</param>
        /// <returns>The fully generated DungeonRoom.</returns>
        private DungeonRoom SetRoomType(Room room, GameObject dungeonRoomObject, int levelNumber)
        {
            switch (room.Type)
            {
                case RoomType.Start:
                    StartRoom startRoom = dungeonRoomObject.AddComponent<StartRoom>();
                    if (Player.LocalPlayer.isServer && levelNumber == 1)
                    {
                        startRoom.SpawnItems(RegionDict.Instance.StartingRoomPickables, DungeonCreator.Instance.TilePositionToWorldPosition(room.Position + (room.Layout.Size / 2)));
                    }
                    return startRoom;

                case RoomType.Combat:
                    CombatRoom combatRoom = dungeonRoomObject.AddComponent<CombatRoom>();
                    combatRoom.ThreatLevel = room.TileCount;
                    combatRoom.enemiesToSpawn = new EnemyObject[combatRoom.ThreatLevel / 48];
                    for (int j = 0; j < combatRoom.enemiesToSpawn.Length; j++)
                    {
                        combatRoom.enemiesToSpawn[j] = RandomUtil.Element(RegionDict.Instance.EnemiesToSpawn);
                    }
                    return combatRoom;

                case RoomType.Loot:
                    LootRoom lootRoom = dungeonRoomObject.AddComponent<LootRoom>();
                    lootRoom.pickables = new Pickable[room.TileCount / 48];
                    for (int j = 0; j < lootRoom.pickables.Length; j++)
                    {
                        lootRoom.pickables[j] = RandomUtil.Element(RegionDict.Instance.LootingRoomPickables);
                    }
                    return lootRoom;

                case RoomType.Shop:
                    ShopRoom shopRoom = dungeonRoomObject.AddComponent<ShopRoom>();
                    const int numItems = 4;
                    shopRoom.shopItems = new Pickable[numItems];
                    shopRoom.locations = new Vector2[numItems];
                    shopRoom.prices = new uint[numItems];
                    // TODO: Replace Pickabledict with dungeon config shop pickables.
                    for (int j = 0; j < numItems; j++)
                    {
                        int rnd = Random.Range(0, PickableDict.Instance.NumWeapons + PickableDict.Instance.NumItems);
                        if (rnd >= PickableDict.Instance.NumWeapons)
                        {
                            shopRoom.shopItems[j] = PickableDict.Instance.GetItem(j - PickableDict.Instance.NumWeapons + 1);
                            shopRoom.prices[j] = 10;
                        }
                        else
                        {
                            shopRoom.shopItems[j] = PickableDict.Instance.GetWeapon(j + 1);
                            shopRoom.prices[j] = 15;
                        }
                        shopRoom.locations[j] = room.Position + new Vector2Int((int)room.GameObjects[j + 4].Position.x, (int)room.GameObjects[j + 4].Position.y);
                    }
                    shopRoom.SpawnItems();
                    return shopRoom;

                case RoomType.Boss:
                    BossRoom bossRoom = dungeonRoomObject.AddComponent<BossRoom>();
                    bossRoom.bossObjects = new BossObject[] { RandomUtil.Element(RegionDict.Instance.BossesToSpawn) };
                    DungeonDict.Instance.SetBossRoom(bossRoom);
                    return bossRoom;

                case RoomType.Empty:
                    EmptyRoom emptyRoom = dungeonRoomObject.AddComponent<EmptyRoom>();
                    return emptyRoom;

                default:
                    throw new System.Exception("Unknown room type! (" + room.Type + ")");
            }
        }

        /// <summary>
        /// Generates each door for the DungeonRoom.
        /// </summary>
        /// <param name="dungeon">The current dungeon.</param>
        /// <param name="prefabDoorLR">The prefab for doors that face either left or right.</param>
        /// <param name="prefabDoorUD">The prefab for doors that face either up or down.</param>
        /// <param name="roomDungeonId">The id of the DungeonRoom to where the doors should be generated to.</param>
        /// <param name="dungeonRoomObject">The GameObject that the dungeon room is generated on.</param>
        /// <param name="dungeonRoom">The DungeonRoom.</param>
        private void SetDoors(Dungeon dungeon, GameObject prefabDoorLR, GameObject prefabDoorUD, int roomDungeonId, GameObject dungeonRoomObject, DungeonRoom dungeonRoom)
        {
            DoorLocations[] doorLocs = dungeon.GetDoorsOfRoom(roomDungeonId);
            for (int i = 0; i < doorLocs.Length; i++)
            {
                DoorLocations doorLoc = doorLocs[i];
                GameObject door = Instantiate(doorLoc.IsLeftRight ? prefabDoorLR : prefabDoorUD,
                    new Vector3(doorLoc.Position.x + 0.5f, doorLoc.Position.y + 0.5f, 0f), Quaternion.identity);

                DungeonDoor dungeonDoor = door.GetComponent<DungeonDoor>();
                dungeonDoor.IsLeftRight = doorLoc.IsLeftRight;

                door.transform.parent = dungeonRoomObject.transform;
                // Is the DungeonRoom the start room?
                if (roomDungeonId == 0)
                    dungeonDoor.IsLocked = false;

                dungeonRoom.doors.Add(dungeonDoor);
            }
        }

        /// <summary>
        /// Sets the Border of the DungeonRoom.
        /// </summary>
        /// <param name="dungeon">The current dungeon.</param>
        /// <param name="i">The index of the current room in the rooms array.</param>
        /// <param name="dr">The current DungeonRoom.</param>
        private void SetRoomBorder(Dungeon dungeon, int i, DungeonRoom dr)
        {
            dr.Border = new Rect(dungeon.Rooms[i].Position.x - 0.5f, dungeon.Rooms[i].Position.y - 0.5f, dungeon.Rooms[i].Layout.XSize + 1, dungeon.Rooms[i].Layout.YSize + 1);
            dr.walkableTiles = dungeon.GetWalkableTiles(i);
        }

        /// <summary>
        /// Sets the Border of a corridor.
        /// </summary>
        /// <param name="corridor">The current Corridor.</param>
        /// <param name="corridorRoom">The current generated CorridorRoom.</param>
        private void SetCorridorBorder(Corridor corridor, CorridorRoom corridorRoom)
        {
            switch (corridor.Direction)
            {
                case Direction.Right:
                    corridorRoom.ForceBorder(new Rect(corridor.Position.x + 1, corridor.Position.y, corridor.Size.x - 1, corridor.Size.y));
                    break;
                case Direction.Left:
                    corridorRoom.ForceBorder(new Rect(corridor.Position.x, corridor.Position.y, corridor.Size.x - 1, corridor.Size.y));
                    break;
                case Direction.Up:
                    corridorRoom.ForceBorder(new Rect(corridor.Position.x, corridor.Position.y + 1, corridor.Size.x, corridor.Size.y - 1));
                    break;
                case Direction.Down:
                    corridorRoom.ForceBorder(new Rect(corridor.Position.x, corridor.Position.y, corridor.Size.x, corridor.Size.y - 1));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Creates all breakables for a dungeon room.
        /// </summary>
        private void SetRoomObjects(Room room, GameObject dungeonRoomPrefab, DungeonRoom dr)
        {
            List<GameObject> objs = new List<GameObject>();
            foreach (var prefabContainer in room.GameObjects)
            {
                GameObject obj = Instantiate(prefabContainer.Prefab);
                obj.transform.position = new Vector3(room.Position.x, room.Position.y, 0f) + prefabContainer.Position;
                obj.transform.parent = dungeonRoomPrefab.transform;
                if (obj.TryGetComponent(out BreakableObject bo))
                    bo.index = Random.Range(0, BreakablesDict.Instance.BreakablesCount);
                objs.Add(obj);
            }
            dr.objects = objs;
        }
    }
}