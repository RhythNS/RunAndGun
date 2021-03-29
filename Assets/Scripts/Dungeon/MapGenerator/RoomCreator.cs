using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator
{
    public class RoomCreator : MonoBehaviour
    {
        public IEnumerator CreateRooms(Dungeon dungeon, int levelNumber, DungeonTimer timer)
        {
            List<DungeonRoom> dungeonRooms = new List<DungeonRoom>();
            DungeonDict.Instance.ResetRooms(dungeon.Rooms.Length);

            GameObject prefabDoorLR = RegionDict.Instance.PrefabDoorLR;
            GameObject prefabDoorUD = RegionDict.Instance.PrefabDoorUD;

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
        }

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
                    for (int j = 0; j < numItems; j++)
                    {
                        int rnd = Random.Range(0, PickableDict.Instance.NumWeapons + PickableDict.Instance.NumItems);
                        if (rnd >= PickableDict.Instance.NumWeapons)
                        {
                            shopRoom.shopItems[j] = PickableDict.Instance.GetItem(j - PickableDict.Instance.NumWeapons);
                            shopRoom.prices[j] = 10;
                        }
                        else
                        {
                            shopRoom.shopItems[j] = PickableDict.Instance.GetWeapon(j);
                            shopRoom.prices[j] = 15;
                        }
                        shopRoom.locations[j] = Vector3.zero;
                    }
                    shopRoom.SpawnItems();
                    return shopRoom;

                case RoomType.Boss:
                    BossRoom bossRoom = dungeonRoomObject.AddComponent<BossRoom>();
                    bossRoom.bossObjects = new BossObject[] { RandomUtil.Element(RegionDict.Instance.BossesToSpawn) };
                    DungeonDict.Instance.SetBossRoom(bossRoom);
                    return bossRoom;

                default:
                    throw new System.Exception("Unknown room type! (" + room.Type + ")");
            }
        }

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
                if (roomDungeonId == 0)
                    dungeonDoor.IsLocked = false;

                dungeonRoom.doors.Add(dungeonDoor);
            }
        }

        private void SetRoomBorder(Dungeon dungeon, int i, DungeonRoom dr)
        {
            dr.Border = new Rect(dungeon.Rooms[i].Position.x, dungeon.Rooms[i].Position.y, dungeon.Rooms[i].Layout.XSize, dungeon.Rooms[i].Layout.YSize);
            dr.walkableTiles = dungeon.GetWalkableTiles(i);
        }

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