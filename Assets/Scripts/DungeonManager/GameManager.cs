using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    private static GameManager instance;

    [SerializeField] private int checkRoomsPerIteration = 3;

    private Coroutine checkForRoomEntered;

    private readonly List<DungeonRoom> rooms = new List<DungeonRoom>();

    private void Awake()
    {
        instance = this;
        if (instance)
        {
            Debug.LogWarning("Already a GameManager in the scene. Deleting myself!");
            Destroy(this);
            return;
        }
    }

    public override void OnStartServer()
    {
        AliveHealthDict.Instance.OnAllPlayersDied += instance.OnAllPlayersDied;
    }

    public static void OnLevelLoaded()
    {
        if (!instance.isServer)
            return;

        instance.checkForRoomEntered = instance.StartCoroutine(instance.CheckForRoomEnter());
    }

    public static void OnLevelCleared()
    {
        if (!instance.isServer)
            return;

        instance.rooms.Clear();
        instance.StopAllCoroutines();

        // load next level
    }

    public static void RegisterRoom(DungeonRoom room)
    {
        if (!instance.isServer)
            return;

        instance.rooms.Add(room);
    }

    public static void OnCombatStarted(Rect bounds)
    {
        if (!instance.isServer)
            return;

        instance.StopCoroutine(instance.checkForRoomEntered);
        // ui.HideAroundBounds(bounds);
    }

    public static void OnCombatEnded()
    {
        if (!instance.isServer)
            return;

        instance.checkForRoomEntered = instance.StartCoroutine(instance.CheckForRoomEnter());
        // ui.StopHidingAroundBounds();
    }

    public void OnAllPlayersDied()
    {
        instance.StopAllCoroutines();

        instance.GameOver();
    }

    [ClientRpc]
    public void GameOver()
    {
        // Display game over ui etc.
    }

    private IEnumerator CheckForRoomEnter()
    {
        while (true)
        {
            int counter = 0;

            List<Health> playerHealths = AliveHealthDict.Instance.PlayerHealths;

            if (playerHealths.Count == 0)
                yield break;

            List<Bounds> playerBounds = new List<Bounds>(playerHealths.Count);

            for (int i = 0; i < playerHealths.Count; i++)
                playerBounds.Add(playerHealths[i].GetComponent<Player>().Collider2D.bounds);

            for (int i = 0; i < rooms.Count; i++)
            {
                // If the room has no event or has an event but is already cleared, skip this room.
                if (!rooms[i].EventOnRoomEntered || rooms[i].AlreadyCleared)
                    continue;


                ++counter;

                if (rooms[i].CheckAllPlayersEntered(playerBounds))
                {
                    rooms[i].OnAllPlayersEntered();
                    yield break;
                }

                // Already checked enough rooms?
                if (counter >= checkRoomsPerIteration)
                    yield return null;
            }

            yield return null;
        }
    }


    private void OnDestroy()
    {
        if (instance == this)
        {
            AliveHealthDict.Instance.OnAllPlayersDied -= OnAllPlayersDied;
            instance = null;
        }
    }
}
