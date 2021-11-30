using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Assets.Scripts.Dungeon.MapGenRework
{
    [Serializable]
    public class RoomCollection
    {
        [SerializeField]
        private GameObject[] roomLayouts;

        public List<GameObject> GetRoomsFromType(params RoomType[] types)
        {
            List<GameObject> rooms = new List<GameObject>();

            for (int i = 0; i < roomLayouts.Length; i++)
                for (int j = 0; j < types.Length; j++)
                    if (roomLayouts[i].GetComponent<Room>().GetRoomType() == types[j])
                        rooms.Add(roomLayouts[i]);

            return rooms;
        }

        public GameObject GetStartRoom()
        {
            return roomLayouts.Where(room => room.GetComponent<Room>().GetRoomType() == RoomType.Start).First();
        }
    }

    public class DungeonCreator : MonoBehaviour
    {
        [SerializeField]
        private RoomCollection roomCollection;

        [SerializeField]
        private int maxRooms = 25;

        private List<GameObject> roomGameObjects = new List<GameObject>();

        private const int MAX_ITERATIONS = 100;

        public void CreateDungeon()
        {
            foreach (var go in roomGameObjects)
            {
#if UNITY_EDITOR
                DestroyImmediate(go);
#else
                Destroy(go);
#endif
            }

            roomGameObjects = new List<GameObject>();

            GameObject startRoomPrefab = roomCollection.GetStartRoom();

            GameObject startRoom = Instantiate(startRoomPrefab, Vector3.zero, Quaternion.identity);

            startRoom.transform.parent = this.transform;
            roomGameObjects.Add(startRoom);

            List<GameObject> corridorPrefabs = roomCollection.GetRoomsFromType(RoomType.Corridor);
            List<GameObject> defaultRoomPrefabs = roomCollection.GetRoomsFromType(RoomType.Combat, RoomType.Loot);

            if (corridorPrefabs.Count == 0)
            {
                Debug.LogError("No corridor prefabs found!");
                return;
            }
            if (defaultRoomPrefabs.Count == 0)
            {
                Debug.LogError("No combat/loot room prefabs found!");
                return;
            }

            int iterations = 0;
            while (roomGameObjects.Count < maxRooms && iterations <= MAX_ITERATIONS)
            {
                iterations++;

                RoomConnection rndConnection = RandomUtil.Element(GetUnoccupiedConnections());
                GameObject rndRoomPrefab = null;
                if (rndConnection.GetComponentInParent<Room>().GetRoomType() != RoomType.Corridor)
                    rndRoomPrefab = RandomUtil.Element<GameObject>(corridorPrefabs);
                else
                    rndRoomPrefab = RandomUtil.Element<GameObject>(defaultRoomPrefabs);

                RoomConnection connection = null;
                int connectionIndex = -1;
                int index = 0;
                foreach (var conn in rndRoomPrefab.GetComponent<Room>().GetConnections())
                {
                    var dir = rndConnection.transform.right + conn.transform.right;
                    if (dir.sqrMagnitude < 0.0001f)
                    {
                        connection = conn;
                        connectionIndex = index;
                        break;
                    }
                    index++;
                }
                if (connection == null)
                    continue;
                
                Vector3 offset = rndRoomPrefab.transform.position - connection.transform.position;
                Vector3 position = rndConnection.transform.position + offset;

                GameObject rndRoom = Instantiate(rndRoomPrefab, position, Quaternion.identity);

                if (IsOverlappingRoom(rndRoom))
                {
#if UNITY_EDITOR
                    DestroyImmediate(rndRoom);
#else
                    Destroy(go);
#endif
                    continue;
                }

                rndRoom.transform.parent = this.transform;

                roomGameObjects.Add(rndRoom);

                rndConnection.isOccupied = true;
                rndRoom.GetComponent<Room>().GetConnections()[connectionIndex].isOccupied = true;
            }

            for (int i = 0; i < roomGameObjects.Count; i++)
            {
                Room room = roomGameObjects[i].GetComponent<Room>();

                if (room.GetRoomType() == RoomType.Corridor && room.GetUnoccupiedConnections().Count > 0)
                {
                    DestroyImmediate(roomGameObjects[i]);
                }
            }
        }

        private bool IsOverlappingRoom(GameObject roomObject)
        {
            Room room = roomObject.GetComponent<Room>();

            for (int i = 0; i < roomGameObjects.Count; i++)
            {
                if (room.IsInBounds(roomGameObjects[i].GetComponent<Room>().GetBoundingRectWorld()))
                {
                    Debug.Log("overlap");
                    return true;
                }
            }

            return false;
        }

        public List<RoomConnection> GetUnoccupiedConnections()
        {
            List<RoomConnection> connections = new List<RoomConnection>();

            foreach (var go in roomGameObjects)
            {
                connections.AddRange(go.GetComponent<Room>().GetUnoccupiedConnections());
            }

            return connections;
        }
    }
}
