using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemySpawner : NetworkBehaviour, IPathfinder
{
    [SerializeField] private EnemyObject[] objectsToSpawn;
    private int currentObjectToSpawn;
    private List<Enemy> spawnedEnemies = new List<Enemy>();
    private ExtendedCoroutine waitForEnemySpawn;

    private List<Player> currentPlayersInside = new List<Player>();

    [SerializeField] private Vector2Int from;
    [SerializeField] private Vector2Int to;

    [SerializeField] private Vector2 spawnPoint;

    public Rect RoomBorder => roomBorder;
    [SerializeField] private Rect roomBorder;

    [SerializeField] private DungeonRadioButtonGroup buttonGroup;

    private bool shouldSpawnReward = false;

    private void Awake()
    {
        Vector2Int size = to - from;
        roomBorder = new Rect(from, size);
    }

    public override void OnStartServer()
    {
        buttonGroup.OnSelectedButtonChanged += OnSelectedButtonChanged;
        OnSelectedButtonChanged(buttonGroup.CurrentOn);
    }

    private void OnSelectedButtonChanged(int number)
    {
        currentObjectToSpawn = number;
    }

    public List<Vector2> TryFindPath(Vector2Int start, Vector2Int destination)
    {
        List<Vector2> path = new List<Vector2>();

        if (VectorUtil.InBounds(from, to, destination) == false)
            return path;

        Vector2Int current = start;
        while (current != destination)
        {
            Vector2Int dif = start - current;
            if (Mathf.Abs(dif.x) > Mathf.Abs(dif.y))
                current = current + new Vector2Int(dif.x > 0 ? 1 : -1, 0);
            else
                current = current + new Vector2Int(0, dif.y > 0 ? 1 : -1);

            path.Add(current);
        }

        return path;
    }

    [Server]
    public void SpawnEnemy()
    {
        if (ArrayUtil.InRange(objectsToSpawn, currentObjectToSpawn) == false)
        {
            Debug.LogWarning("Index out of bounds: " + currentObjectToSpawn + "!");
            return;
        }

        Enemy enemy = Enemy.InstantiateAndSpawn(objectsToSpawn[currentObjectToSpawn], roomBorder, spawnPoint, Quaternion.identity);
        spawnedEnemies.Add(enemy);
        enemy.Health.OnDied += EnemyDied;

        shouldSpawnReward = true;
    }

    [Server]
    private void EnemyDied(GameObject diedEntity)
    {
        Enemy enemy = diedEntity.gameObject.GetComponent<Enemy>();
        if (spawnedEnemies.Contains(enemy) == false)
            return;

        enemy.Health.OnDied -= EnemyDied;
        spawnedEnemies.Remove(enemy);

        if (spawnedEnemies.Count == 0)
            AllEnemiesDied();
    }

    [Server]
    private void AllEnemiesDied()
    {
        if (shouldSpawnReward == false)
            return;

        // Spawn reward
        Debug.Log("Spawn reward here");
    }

    [Server]
    public void DeSpawnEnemy()
    {
        shouldSpawnReward = false;
        for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
        {
            spawnedEnemies[i].Health.Kill();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isServer == false)
            return;

        if (other.TryGetComponent(out Player player) == false)
            return;

        if (currentPlayersInside.Contains(player) == true)
            return;

        currentPlayersInside.Add(player);

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isServer == false)
            return;

        if (other.TryGetComponent(out Player player) == false)
            return;

        if (currentPlayersInside.Contains(player) == false)
            return;

        currentPlayersInside.Remove(player);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 drawFrom = new Vector3(from.x, from.y);
        Vector3 drawTo = new Vector3(to.x, to.y);

        Vector3 size = drawTo - drawFrom;
        Vector3 mid = drawFrom + size * 0.5f;

        Gizmos.DrawWireCube(mid, size);

        GizmosUtil.DrawPoint(spawnPoint);
    }
}
