using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adds dungeon rooms to the DungeonDict and call OnAllPlayersEntered when each player
/// entered a dungeon room.
/// </summary>
public class RoomTester : MonoBehaviour
{
    [SerializeField] private DungeonRoom[] roomsToAdd;

    private void Start()
    {
        DungeonDict.Instance.ResetRooms(roomsToAdd.Length);
        for (int i = 0; i < roomsToAdd.Length; i++)
        {
            DungeonDict.Instance.Register(roomsToAdd[i]);
        }
        StartCoroutine(CheckForRoomEnter());
    }

    private IEnumerator CheckForRoomEnter()
    {
        while (true)
        {
            int counter = 0;

            List<Health> playerHealths = AliveHealthDict.Instance.PlayerHealths;

            while (playerHealths.Count == 0)
                yield return new WaitForSeconds(1.0f);

            List<Bounds> playerBounds = new List<Bounds>(playerHealths.Count);

            for (int i = 0; i < playerHealths.Count; i++)
                playerBounds.Add(playerHealths[i].GetComponent<Player>().Collider2D.bounds);

            DungeonRoom[] rooms = DungeonDict.Instance.Rooms;

            for (int i = 0; i < rooms.Length; i++)
            {
                if (!rooms[i])
                    break;

                // If the room has no event or has an event but is already cleared, skip this room.
                if (!rooms[i].EventOnRoomEntered || rooms[i].AlreadyCleared)
                    continue;

                ++counter;

                if (rooms[i].CheckAllPlayersEntered(playerBounds))
                {
                    Debug.Log("Starting room event!");
                    rooms[i].OnAllPlayersEntered();
                    yield break;
                }

                // Already checked enough rooms?
                if (counter >= 3)
                    yield return null;
            }

            yield return null;
        }
    }
}
